using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.PurchaseRequisitions;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.Rfqs;
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

[PermissionAuthorize(PermissionNames.RfqView)]
public class RfqsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateRfqDto> _createValidator;
    private readonly IValidator<UpdateRfqDto> _updateValidator;

    public RfqsController(IMediator mediator, IValidator<CreateRfqDto> createValidator, IValidator<UpdateRfqDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? rfqNumber = null, long? projectId = null, RfqStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var rfqs = await _mediator.Send(new GetAllRfqsQuery(page, pageSize, rfqNumber, projectId, status, dateFrom, dateTo));

        ViewBag.RfqNumber = rfqNumber;
        ViewBag.ProjectId = projectId;
        ViewBag.Status = status;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);
        ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(RfqStatus)).Cast<RfqStatus>().Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name", status);

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", rfqs);
        }

        return View(rfqs);
    }

    [PermissionAuthorize(PermissionNames.RfqManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateRfqDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RfqManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRfqDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateRfqCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.RfqManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var rfq = await _mediator.Send(new GetRfqByIdQuery(id));
        if (rfq is null)
        {
            return NotFound();
        }

        var dto = new UpdateRfqDto
        {
            Id = rfq.Id,
            RfqNumber = rfq.RfqNumber,
            IssueDate = rfq.IssueDate,
            ClosingDate = rfq.ClosingDate,
            Status = rfq.Status,
            PurchaseRequisitionId = rfq.PurchaseRequisitionId,
            SupplierIds = rfq.Vendors.Select(v => v.SupplierId).ToList(),
            Details = rfq.Details.Select(d => new CreateRfqDetailDto
            {
                MaterialId = d.MaterialId,
                Quantity = d.Quantity,
                UnitOfMeasure = d.UnitOfMeasure,
                Specification = d.Specification
            }).ToList()
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RfqManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateRfqDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var result = await _mediator.Send(new UpdateRfqCommand(dto));
        if (result == UpdateRfqResult.NotFound)
        {
            return NotFound();
        }

        if (result == UpdateRfqResult.Locked)
        {
            ModelState.AddModelError(string.Empty, "This RFQ is closed and cannot be edited.");
            await PopulateDropdownsAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [PermissionAuthorize(PermissionNames.RfqManage)]
    public async Task<IActionResult> GetPurchaseRequisitionDetails(long id)
    {
        var requisition = await _mediator.Send(new GetPurchaseRequisitionByIdQuery(id));
        if (requisition is null)
        {
            return NotFound();
        }

        var details = requisition.Details.Select(d => new
        {
            materialId = d.MaterialId,
            quantity = d.Quantity,
            unitOfMeasure = (int)d.UnitOfMeasure,
            specification = d.Remarks
        });

        return Json(details);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RfqManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetRfqActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var requisitions = await _mediator.Send(new GetAllPurchaseRequisitionsQuery(PageSize: int.MaxValue));
        ViewBag.PurchaseRequisitions = requisitions.Items.Select(r => new SelectListItem
        {
            Value = r.Id.ToString(),
            Text = $"{r.RequisitionNumber} | {r.ProjectName}"
        }).ToList();

        var suppliers = await _mediator.Send(new GetAllSuppliersQuery(PageSize: int.MaxValue));
        ViewBag.Suppliers = suppliers.Items;

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
