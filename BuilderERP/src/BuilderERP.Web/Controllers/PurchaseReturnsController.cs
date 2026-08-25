using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.GoodsReceives;
using BuilderERP.Application.Features.PurchaseReturns;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.PurchaseReturnView)]
public class PurchaseReturnsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreatePurchaseReturnDto> _createValidator;
    private readonly IValidator<UpdatePurchaseReturnDto> _updateValidator;

    public PurchaseReturnsController(IMediator mediator, IValidator<CreatePurchaseReturnDto> createValidator, IValidator<UpdatePurchaseReturnDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var returns = await _mediator.Send(new GetAllPurchaseReturnsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", returns);
        }

        return View(returns);
    }

    [PermissionAuthorize(PermissionNames.PurchaseReturnManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreatePurchaseReturnDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PurchaseReturnManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePurchaseReturnDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreatePurchaseReturnCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.PurchaseReturnManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var purchaseReturn = await _mediator.Send(new GetPurchaseReturnByIdQuery(id));
        if (purchaseReturn is null)
        {
            return NotFound();
        }

        var dto = new UpdatePurchaseReturnDto
        {
            Id = purchaseReturn.Id,
            ReturnNumber = purchaseReturn.ReturnNumber,
            ReturnDate = purchaseReturn.ReturnDate,
            Reason = purchaseReturn.Reason,
            Status = purchaseReturn.Status,
            GoodsReceiveId = purchaseReturn.GoodsReceiveId,
            Details = purchaseReturn.Details.Select(d => new CreatePurchaseReturnDetailDto
            {
                GoodsReceiveDetailId = d.GoodsReceiveDetailId,
                MaterialId = d.MaterialId,
                ReturnQuantity = d.ReturnQuantity,
                UnitOfMeasure = d.UnitOfMeasure,
                UnitPrice = d.UnitPrice,
                VatPercent = d.VatPercent,
                TaxPercent = d.TaxPercent
            }).ToList()
        };

        ViewBag.DetailMaterialNames = purchaseReturn.Details.Select(d => d.MaterialName).ToList();

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PurchaseReturnManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdatePurchaseReturnDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var result = await _mediator.Send(new UpdatePurchaseReturnCommand(dto));
        if (result == UpdatePurchaseReturnResult.NotFound)
        {
            return NotFound();
        }

        if (result == UpdatePurchaseReturnResult.Locked)
        {
            ModelState.AddModelError(string.Empty, "This return is already approved or completed and cannot be edited.");
            await PopulateDropdownsAsync();
            return View(dto);
        }

        if (result == UpdatePurchaseReturnResult.OverReturn)
        {
            ModelState.AddModelError(string.Empty, "One or more lines exceed the received quantity minus quantity already returned.");
            await PopulateDropdownsAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PurchaseReturnManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetPurchaseReturnActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [PermissionAuthorize(PermissionNames.PurchaseReturnView)]
    public async Task<IActionResult> GetReceivedLines(long goodsReceiveId)
    {
        var receive = await _mediator.Send(new GetGoodsReceiveByIdQuery(goodsReceiveId));
        if (receive is null)
        {
            return NotFound();
        }

        var lines = receive.Details.Select(d => new
        {
            goodsReceiveDetailId = d.Id,
            materialId = d.MaterialId,
            materialName = d.MaterialName,
            unitOfMeasure = (int)d.UnitOfMeasure,
            receivedQuantity = d.ReceivedQuantity,
            unitPrice = d.UnitPrice
        });

        return Json(lines);
    }

    private async Task PopulateDropdownsAsync()
    {
        var receives = await _mediator.Send(new GetAllGoodsReceivesQuery(PageSize: int.MaxValue));
        ViewBag.GoodsReceives = new SelectList(receives.Items, "Id", "GrnNumber");
    }
}
