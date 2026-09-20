using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.CashPurchaseOrders;
using BuilderERP.Application.Features.EngineerWorkOrders;
using BuilderERP.Application.Features.GoodsReceives;
using BuilderERP.Application.Features.GoodsReceives.Export;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.PurchaseOrders;
using BuilderERP.Application.Features.Warehouses;
using BuilderERP.Domain.Enums;
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
    private readonly GoodsReceivePdfExporter _pdfExporter;

    public GoodsReceivesController(
        IMediator mediator,
        IValidator<CreateGoodsReceiveDto> createValidator,
        IValidator<UpdateGoodsReceiveDto> updateValidator,
        GoodsReceivePdfExporter pdfExporter)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _pdfExporter = pdfExporter;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? grnNumber = null, long? projectId = null, GrnStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var receives = await _mediator.Send(new GetAllGoodsReceivesQuery(page, pageSize, grnNumber, projectId, status, dateFrom, dateTo));

        ViewBag.GrnNumber = grnNumber;
        ViewBag.ProjectId = projectId;
        ViewBag.Status = status;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);
        ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(GrnStatus)).Cast<GrnStatus>().Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name", status);

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
            SourceType = receive.SourceType,
            PurchaseOrderId = receive.PurchaseOrderId,
            EngineerWorkOrderId = receive.EngineerWorkOrderId,
            CashPurchaseOrderId = receive.CashPurchaseOrderId,
            WarehouseId = receive.WarehouseId,
            Details = receive.Details.Select(d => new CreateGoodsReceiveDetailDto
            {
                PurchaseOrderDetailId = d.PurchaseOrderDetailId,
                EngineerWorkOrderDetailId = d.EngineerWorkOrderDetailId,
                CashPurchaseOrderDetailId = d.CashPurchaseOrderDetailId,
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
            await PopulateDetailMaterialNamesAsync(dto);
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
            await PopulateDetailMaterialNamesAsync(dto);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        if (result == UpdateGoodsReceiveResult.OverReceipt)
        {
            ModelState.AddModelError(string.Empty, "One or more lines exceed the remaining ordered quantity plus the allowed over-receipt tolerance.");
            await PopulateDetailMaterialNamesAsync(dto);
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

    public async Task<IActionResult> Print(long id)
    {
        var data = await _mediator.Send(new GetGoodsReceivePrintDataQuery(id));
        if (data is null)
        {
            return NotFound();
        }

        data.PrintedBy = User.Identity?.Name;
        data.PrintedAt = DateTime.Now;

        return View(data);
    }

    public async Task<IActionResult> PrintPdf(long id)
    {
        var data = await _mediator.Send(new GetGoodsReceivePrintDataQuery(id));
        if (data is null)
        {
            return NotFound();
        }

        data.PrintedBy = User.Identity?.Name;
        data.PrintedAt = DateTime.Now;

        var pdfBytes = _pdfExporter.Export(data);
        return File(pdfBytes, "application/pdf", $"{data.GrnNumber}.pdf");
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
            detailId = d.Id,
            materialId = d.MaterialId,
            materialName = d.MaterialName,
            unitOfMeasure = (int)d.UnitOfMeasure,
            remainingQuantity = d.RemainingQuantity,
            unitPrice = d.UnitPrice
        });

        return Json(lines);
    }

    [HttpGet]
    [PermissionAuthorize(PermissionNames.GoodsReceiveView)]
    public async Task<IActionResult> GetOpenEngineerWorkOrderLines(long engineerWorkOrderId)
    {
        var order = await _mediator.Send(new GetEngineerWorkOrderByIdQuery(engineerWorkOrderId));
        if (order is null)
        {
            return NotFound();
        }

        var lines = order.Details.Select(d => new
        {
            detailId = d.Id,
            materialId = d.MaterialId,
            materialName = d.MaterialName,
            unitOfMeasure = (int)d.UnitOfMeasure,
            remainingQuantity = d.RemainingQuantity,
            unitPrice = d.Rate
        });

        return Json(lines);
    }

    [HttpGet]
    [PermissionAuthorize(PermissionNames.GoodsReceiveView)]
    public async Task<IActionResult> GetOpenCashPurchaseOrderLines(long cashPurchaseOrderId)
    {
        var order = await _mediator.Send(new GetCashPurchaseOrderByIdQuery(cashPurchaseOrderId));
        if (order is null)
        {
            return NotFound();
        }

        var lines = order.Details.Select(d => new
        {
            detailId = d.Id,
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
        ViewBag.PurchaseOrders = orders.Items.Select(o => new SelectListItem
        {
            Value = o.Id.ToString(),
            Text = $"{o.PONumber} | {o.ProjectName}"
        }).ToList();

        var workOrders = await _mediator.Send(new GetAllEngineerWorkOrdersQuery(PageSize: int.MaxValue));
        ViewBag.EngineerWorkOrders = workOrders.Items.Select(o => new SelectListItem
        {
            Value = o.Id.ToString(),
            Text = $"{o.WorkOrderNo} | {o.SupplierName}"
        }).ToList();

        var cashOrders = await _mediator.Send(new GetAllCashPurchaseOrdersQuery(PageSize: int.MaxValue));
        ViewBag.CashPurchaseOrders = cashOrders.Items.Select(o => new SelectListItem
        {
            Value = o.Id.ToString(),
            Text = $"{o.CPONumber} | {o.ProjectName}"
        }).ToList();

        var warehouses = await _mediator.Send(new GetAllWarehousesQuery(PageSize: int.MaxValue));
        ViewBag.Warehouses = warehouses.Items.Select(w => new SelectListItem
        {
            Value = w.Id.ToString(),
            Text = $"{w.Name} | {w.ProjectName}"
        }).ToList();
    }

    private async Task PopulateDetailMaterialNamesAsync(UpdateGoodsReceiveDto dto)
    {
        var namesById = new Dictionary<long, string>();
        var names = new List<string>();

        foreach (var detail in dto.Details)
        {
            if (!namesById.TryGetValue(detail.MaterialId, out var name))
            {
                var material = await _mediator.Send(new GetMaterialByIdQuery(detail.MaterialId));
                name = material?.Name ?? string.Empty;
                namesById[detail.MaterialId] = name;
            }

            names.Add(name);
        }

        ViewBag.DetailMaterialNames = names;
    }
}
