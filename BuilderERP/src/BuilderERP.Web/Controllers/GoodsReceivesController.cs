using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.GoodsReceives;
using BuilderERP.Application.Features.PurchaseOrders;
using BuilderERP.Application.Features.Warehouses;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.GoodsReceiveView)]
public class GoodsReceivesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateGoodsReceiveDto> _createValidator;
    private readonly IValidator<UpdateGoodsReceiveDto> _updateValidator;

    public GoodsReceivesController(IMediator mediator, IValidator<CreateGoodsReceiveDto> createValidator, IValidator<UpdateGoodsReceiveDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var receives = await _mediator.Send(new GetAllGoodsReceivesQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", receives);
        }

        return View(receives);
    }

    [PermissionAuthorize(PermissionNames.GoodsReceiveManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateGoodsReceiveDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.GoodsReceiveManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGoodsReceiveDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateGoodsReceiveCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.GoodsReceiveManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var receive = await _mediator.Send(new GetGoodsReceiveByIdQuery(id));
        if (receive is null)
        {
            return NotFound();
        }

        var dto = new UpdateGoodsReceiveDto
        {
            Id = receive.Id,
            GrnNumber = receive.GrnNumber,
            ReceivedDate = receive.ReceivedDate,
            Remarks = receive.Remarks,
            Status = receive.Status,
            PurchaseOrderId = receive.PurchaseOrderId,
            WarehouseId = receive.WarehouseId,
            Details = receive.Details.Select(d => new CreateGoodsReceiveDetailDto
            {
                PurchaseOrderDetailId = d.PurchaseOrderDetailId,
                MaterialId = d.MaterialId,
                ReceivedQuantity = d.ReceivedQuantity,
                UnitOfMeasure = d.UnitOfMeasure,
                UnitPrice = d.UnitPrice,
                BatchNo = d.BatchNo,
                SerialNo = d.SerialNo,
                VatPercent = d.VatPercent,
                TaxPercent = d.TaxPercent
            }).ToList()
        };

        ViewBag.DetailMaterialNames = receive.Details.Select(d => d.MaterialName).ToList();

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.GoodsReceiveManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateGoodsReceiveDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var result = await _mediator.Send(new UpdateGoodsReceiveCommand(dto));
        if (result == UpdateGoodsReceiveResult.NotFound)
        {
            return NotFound();
        }

        if (result == UpdateGoodsReceiveResult.Locked)
        {
            ModelState.AddModelError(string.Empty, "This GRN is already approved and cannot be edited.");
            await PopulateDropdownsAsync();
            return View(dto);
        }

        if (result == UpdateGoodsReceiveResult.OverReceipt)
        {
            ModelState.AddModelError(string.Empty, "One or more lines exceed the remaining ordered quantity plus the allowed over-receipt tolerance.");
            await PopulateDropdownsAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.GoodsReceiveManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetGoodsReceiveActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [PermissionAuthorize(PermissionNames.GoodsReceiveView)]
    public async Task<IActionResult> GetOpenPurchaseOrderLines(long purchaseOrderId)
    {
        var order = await _mediator.Send(new GetPurchaseOrderByIdQuery(purchaseOrderId));
        if (order is null)
        {
            return NotFound();
        }

        var lines = order.Details.Select(d => new
        {
            purchaseOrderDetailId = d.Id,
            materialId = d.MaterialId,
            materialName = d.MaterialName,
            unitOfMeasure = (int)d.UnitOfMeasure,
            remainingQuantity = d.RemainingQuantity,
            unitPrice = d.UnitPrice
        });

        return Json(lines);
    }

    private async Task PopulateDropdownsAsync()
    {
        var orders = await _mediator.Send(new GetAllPurchaseOrdersQuery(PageSize: int.MaxValue));
        ViewBag.PurchaseOrders = new SelectList(orders.Items, "Id", "PONumber");

        var warehouses = await _mediator.Send(new GetAllWarehousesQuery(PageSize: int.MaxValue));
        ViewBag.Warehouses = new SelectList(warehouses.Items, "Id", "Name");
    }
}
