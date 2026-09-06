using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.PurchaseOrders;
using BuilderERP.Application.Features.PurchaseOrders.Export;
using BuilderERP.Application.Features.VendorQuotations;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.PurchaseOrderView)]
public class PurchaseOrdersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreatePurchaseOrderDto> _createValidator;
    private readonly IValidator<UpdatePurchaseOrderDto> _updateValidator;
    private readonly PurchaseOrderPdfExporter _pdfExporter;

    public PurchaseOrdersController(
        IMediator mediator,
        IValidator<CreatePurchaseOrderDto> createValidator,
        IValidator<UpdatePurchaseOrderDto> updateValidator,
        PurchaseOrderPdfExporter pdfExporter)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _pdfExporter = pdfExporter;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var orders = await _mediator.Send(new GetAllPurchaseOrdersQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", orders);
        }

        return View(orders);
    }

    [PermissionAuthorize(PermissionNames.PurchaseOrderManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreatePurchaseOrderDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PurchaseOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePurchaseOrderDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreatePurchaseOrderCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.PurchaseOrderManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var order = await _mediator.Send(new GetPurchaseOrderByIdQuery(id));
        if (order is null)
        {
            return NotFound();
        }

        var dto = new UpdatePurchaseOrderDto
        {
            Id = order.Id,
            PONumber = order.PONumber,
            OrderDate = order.OrderDate,
            DeliveryDate = order.DeliveryDate,
            Status = order.Status,
            TermsOfPayment = order.TermsOfPayment,
            DispatchedThrough = order.DispatchedThrough,
            Destination = order.Destination,
            Remarks = order.Remarks,
            VendorQuotationId = order.VendorQuotationId,
            Details = order.Details.Select(d => new CreatePurchaseOrderDetailDto
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
    [PermissionAuthorize(PermissionNames.PurchaseOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdatePurchaseOrderDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var result = await _mediator.Send(new UpdatePurchaseOrderCommand(dto));
        if (result == UpdatePurchaseOrderResult.NotFound)
        {
            return NotFound();
        }

        if (result == UpdatePurchaseOrderResult.Locked)
        {
            ModelState.AddModelError(string.Empty, "This purchase order is no longer in Draft status and cannot be edited.");
            await PopulateDropdownsAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [PermissionAuthorize(PermissionNames.PurchaseOrderManage)]
    public async Task<IActionResult> GetVendorQuotationDetails(long id)
    {
        var quotation = await _mediator.Send(new GetVendorQuotationByIdQuery(id));
        if (quotation is null)
        {
            return NotFound();
        }

        var details = quotation.Details.Select(d => new
        {
            materialId = d.MaterialId,
            orderedQuantity = d.Quantity,
            unitOfMeasure = (int)d.UnitOfMeasure,
            unitPrice = d.UnitPrice,
            discountPercent = d.DiscountPercent,
            vatPercent = d.VatPercent,
            taxPercent = d.TaxPercent
        });

        return Json(details);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PurchaseOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetPurchaseOrderActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Print(long id)
    {
        var data = await _mediator.Send(new GetPurchaseOrderPrintDataQuery(id));
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
        var data = await _mediator.Send(new GetPurchaseOrderPrintDataQuery(id));
        if (data is null)
        {
            return NotFound();
        }

        data.PrintedBy = User.Identity?.Name;
        data.PrintedAt = DateTime.Now;

        var pdfBytes = _pdfExporter.Export(data);
        return File(pdfBytes, "application/pdf", $"{data.PONumber}.pdf");
    }

    private async Task PopulateDropdownsAsync()
    {
        var quotations = await _mediator.Send(new GetAllVendorQuotationsQuery(PageSize: int.MaxValue));
        ViewBag.VendorQuotations = quotations.Items.Select(q => new SelectListItem
        {
            Value = q.Id.ToString(),
            Text = $"{q.QuotationNumber} | {q.ProjectName}"
        }).ToList();

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
