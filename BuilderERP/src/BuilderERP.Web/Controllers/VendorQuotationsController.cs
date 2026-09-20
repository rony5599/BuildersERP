using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.Rfqs;
using BuilderERP.Application.Features.Suppliers;
using BuilderERP.Application.Features.VendorQuotations;
using BuilderERP.Application.Features.VendorQuotations.Export;
using BuilderERP.Domain.Enums;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.VendorQuotationView)]
public class VendorQuotationsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateVendorQuotationDto> _createValidator;
    private readonly IValidator<UpdateVendorQuotationDto> _updateValidator;
    private readonly VendorQuotationPdfExporter _pdfExporter;

    public VendorQuotationsController(
        IMediator mediator,
        IValidator<CreateVendorQuotationDto> createValidator,
        IValidator<UpdateVendorQuotationDto> updateValidator,
        VendorQuotationPdfExporter pdfExporter)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _pdfExporter = pdfExporter;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? quotationNumber = null, long? projectId = null, VendorQuotationStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var quotations = await _mediator.Send(new GetAllVendorQuotationsQuery(page, pageSize, quotationNumber, projectId, status, dateFrom, dateTo));

        ViewBag.QuotationNumber = quotationNumber;
        ViewBag.ProjectId = projectId;
        ViewBag.Status = status;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);
        ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(VendorQuotationStatus)).Cast<VendorQuotationStatus>().Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name", status);

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", quotations);
        }

        return View(quotations);
    }

    [PermissionAuthorize(PermissionNames.VendorQuotationManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateVendorQuotationDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.VendorQuotationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVendorQuotationDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateVendorQuotationCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.VendorQuotationManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var quotation = await _mediator.Send(new GetVendorQuotationByIdQuery(id));
        if (quotation is null)
        {
            return NotFound();
        }

        var dto = new UpdateVendorQuotationDto
        {
            Id = quotation.Id,
            QuotationNumber = quotation.QuotationNumber,
            QuotationDate = quotation.QuotationDate,
            DeliveryDays = quotation.DeliveryDays,
            Status = quotation.Status,
            RfqId = quotation.RfqId,
            SupplierId = quotation.SupplierId,
            Details = quotation.Details.Select(d => new CreateVendorQuotationDetailDto
            {
                MaterialId = d.MaterialId,
                Quantity = d.Quantity,
                UnitOfMeasure = d.UnitOfMeasure,
                UnitPrice = d.UnitPrice,
                DiscountPercent = d.DiscountPercent,
                VatPercent = d.VatPercent,
                TaxPercent = d.TaxPercent,
                DeliveryDays = d.DeliveryDays
            }).ToList()
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.VendorQuotationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateVendorQuotationDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var result = await _mediator.Send(new UpdateVendorQuotationCommand(dto));
        if (result == UpdateVendorQuotationResult.NotFound)
        {
            return NotFound();
        }

        if (result == UpdateVendorQuotationResult.Locked)
        {
            ModelState.AddModelError(string.Empty, "This quotation has already been selected or rejected and cannot be edited.");
            await PopulateDropdownsAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [PermissionAuthorize(PermissionNames.VendorQuotationManage)]
    public async Task<IActionResult> GetRfqDetails(long id)
    {
        var rfq = await _mediator.Send(new GetRfqByIdQuery(id));
        if (rfq is null)
        {
            return NotFound();
        }

        var details = rfq.Details.Select(d => new
        {
            materialId = d.MaterialId,
            quantity = d.Quantity,
            unitOfMeasure = (int)d.UnitOfMeasure,
            specification = d.Specification
        });

        return Json(details);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.VendorQuotationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetVendorQuotationActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Print(long id)
    {
        var data = await _mediator.Send(new GetVendorQuotationPrintDataQuery(id));
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
        var data = await _mediator.Send(new GetVendorQuotationPrintDataQuery(id));
        if (data is null)
        {
            return NotFound();
        }

        data.PrintedBy = User.Identity?.Name;
        data.PrintedAt = DateTime.Now;

        var pdfBytes = _pdfExporter.Export(data);
        return File(pdfBytes, "application/pdf", $"{data.QuotationNumber}.pdf");
    }

    public async Task<IActionResult> Compare(long requisitionId)
    {
        var quotations = await _mediator.Send(new GetVendorQuotationsByRequisitionIdQuery(requisitionId));
        ViewBag.PurchaseRequisitionId = requisitionId;
        return View(quotations);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.VendorQuotationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SelectWinner(long id, long requisitionId)
    {
        await _mediator.Send(new SelectWinnerVendorQuotationCommand(id));
        return RedirectToAction(nameof(Compare), new { requisitionId });
    }

    private async Task PopulateDropdownsAsync()
    {
        var rfqs = await _mediator.Send(new GetAllRfqsQuery(PageSize: int.MaxValue));
        ViewBag.Rfqs = rfqs.Items.Select(r => new SelectListItem
        {
            Value = r.Id.ToString(),
            Text = $"{r.RfqNumber} | {r.RequisitionNumber} | {r.ProjectName}"
        }).ToList();

        var suppliers = await _mediator.Send(new GetAllSuppliersQuery(PageSize: int.MaxValue));
        ViewBag.Suppliers = new SelectList(suppliers.Items, "Id", "Name");

        var materials = await _mediator.Send(new GetAllMaterialsQuery(PageSize: int.MaxValue));
        // Format materials with Code | Name | Category for multicolumn dropdown
        var formattedMaterials = materials.Items.Select(m => new SelectListItem
        {
            Value = m.Id.ToString(),
            Text = $"{m.MaterialCode} | {m.Name} | {m.CategoryName}"
        }).ToList();
        ViewBag.Materials = formattedMaterials;
    }
}
