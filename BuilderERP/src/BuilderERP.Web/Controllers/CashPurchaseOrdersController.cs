using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.CashPurchaseOrders;
using BuilderERP.Application.Features.CashPurchaseOrders.Export;
using BuilderERP.Application.Features.CashRequisitions;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.Suppliers;
using BuilderERP.Domain.Enums;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.CashPurchaseOrderView)]
public class CashPurchaseOrdersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateCashPurchaseOrderDto> _createValidator;
    private readonly IValidator<UpdateCashPurchaseOrderDto> _updateValidator;
    private readonly CashPurchaseOrderPdfExporter _pdfExporter;

    public CashPurchaseOrdersController(
        IMediator mediator,
        IValidator<CreateCashPurchaseOrderDto> createValidator,
        IValidator<UpdateCashPurchaseOrderDto> updateValidator,
        CashPurchaseOrderPdfExporter pdfExporter)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _pdfExporter = pdfExporter;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? cpoNumber = null, long? projectId = null, PurchaseOrderStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var orders = await _mediator.Send(new GetAllCashPurchaseOrdersQuery(page, pageSize, cpoNumber, projectId, status, dateFrom, dateTo));

        ViewBag.CpoNumber = cpoNumber;
        ViewBag.ProjectId = projectId;
        ViewBag.Status = status;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);
        ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(PurchaseOrderStatus)).Cast<PurchaseOrderStatus>().Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name", status);

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", orders);
        }

        return View(orders);
    }

    [PermissionAuthorize(PermissionNames.CashPurchaseOrderManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateCashPurchaseOrderDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashPurchaseOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCashPurchaseOrderDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateCashPurchaseOrderCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.CashPurchaseOrderManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var order = await _mediator.Send(new GetCashPurchaseOrderByIdQuery(id));
        if (order is null)
        {
            return NotFound();
        }

        var dto = new UpdateCashPurchaseOrderDto
        {
            Id = order.Id,
            CPONumber = order.CPONumber,
            OrderDate = order.OrderDate,
            DeliveryDate = order.DeliveryDate,
            Status = order.Status,
            TermsOfPayment = order.TermsOfPayment,
            DispatchedThrough = order.DispatchedThrough,
            Destination = order.Destination,
            Remarks = order.Remarks,
            SupplierId = order.SupplierId,
            CashRequisitionId = order.CashRequisitionId,
            Details = order.Details.Select(d => new CreateCashPurchaseOrderDetailDto
            {
                MaterialId = d.MaterialId,
                OrderedQuantity = d.OrderedQuantity,
                UnitOfMeasure = d.UnitOfMeasure,
                UnitPrice = d.UnitPrice,
                DiscountPercent = d.DiscountPercent,
                VatPercent = d.VatPercent,
                TaxPercent = d.TaxPercent
            }).ToList()
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashPurchaseOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateCashPurchaseOrderDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var result = await _mediator.Send(new UpdateCashPurchaseOrderCommand(dto));
        if (result == UpdateCashPurchaseOrderResult.NotFound)
        {
            return NotFound();
        }

        if (result == UpdateCashPurchaseOrderResult.Locked)
        {
            ModelState.AddModelError(string.Empty, "This cash purchase order is no longer in Draft status and cannot be edited.");
            await PopulateDropdownsAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [PermissionAuthorize(PermissionNames.CashPurchaseOrderManage)]
    public async Task<IActionResult> GetCashRequisitionDetails(long id)
    {
        var requisition = await _mediator.Send(new GetCashRequisitionByIdQuery(id));
        if (requisition is null)
        {
            return NotFound();
        }

        var details = requisition.Details.Select(d => new
        {
            materialId = d.MaterialId,
            orderedQuantity = d.Quantity,
            unitOfMeasure = (int)d.UnitOfMeasure,
            unitPrice = d.EstimatedUnitPrice,
            discountPercent = 0m,
            vatPercent = 0m,
            taxPercent = 0m
        });

        return Json(details);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashPurchaseOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetCashPurchaseOrderActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Print(long id)
    {
        var data = await _mediator.Send(new GetCashPurchaseOrderPrintDataQuery(id));
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
        var data = await _mediator.Send(new GetCashPurchaseOrderPrintDataQuery(id));
        if (data is null)
        {
            return NotFound();
        }

        data.PrintedBy = User.Identity?.Name;
        data.PrintedAt = DateTime.Now;

        var pdfBytes = _pdfExporter.Export(data);
        return File(pdfBytes, "application/pdf", $"{data.CPONumber}.pdf");
    }

    private async Task PopulateDropdownsAsync()
    {
        var requisitions = await _mediator.Send(new GetAllCashRequisitionsQuery(PageSize: int.MaxValue));
        ViewBag.CashRequisitions = requisitions.Items.Select(r => new SelectListItem
        {
            Value = r.Id.ToString(),
            Text = $"{r.RequisitionNumber} | {r.ProjectName}"
        }).ToList();

        var suppliers = await _mediator.Send(new GetAllSuppliersQuery(PageSize: int.MaxValue));
        ViewBag.Suppliers = new SelectList(suppliers.Items, "Id", "Name");

        var materials = await _mediator.Send(new GetAllMaterialsQuery(PageSize: int.MaxValue));
        // Format materials with Code - Name | Category for multicolumn dropdown
        var formattedMaterials = materials.Items.Select(m => new SelectListItem
        {
            Value = m.Id.ToString(),
            Text = $"{m.MaterialCode} | {m.Name} | {m.CategoryName}"
        }).ToList();
        ViewBag.Materials = formattedMaterials;
    }
}
