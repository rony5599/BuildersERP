using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.CashRequisitions;
using BuilderERP.Application.Features.CashRequisitions.Export;
using BuilderERP.Application.Features.Departments;
using BuilderERP.Application.Features.Employees;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Infrastructure.Persistence;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.CashRequisitionView)]
public class CashRequisitionsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateCashRequisitionDto> _createValidator;
    private readonly IValidator<UpdateCashRequisitionDto> _updateValidator;
    private readonly CashRequisitionPdfExporter _pdfExporter;
    private readonly AppDbContext _db;

    public CashRequisitionsController(
        IMediator mediator,
        IValidator<CreateCashRequisitionDto> createValidator,
        IValidator<UpdateCashRequisitionDto> updateValidator,
        CashRequisitionPdfExporter pdfExporter, AppDbContext db)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _pdfExporter = pdfExporter;
        _db = db;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? requisitionNumber = null, long? projectId = null, RequisitionStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        ViewBag.ActionAssignment = await GetCurrentAssignmentAsync();
        var requisitions = await _mediator.Send(new GetAllCashRequisitionsQuery(page, pageSize, requisitionNumber, projectId, status, dateFrom, dateTo));

        ViewBag.RequisitionNumber = requisitionNumber;
        ViewBag.ProjectId = projectId;
        ViewBag.Status = status;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);
        ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(RequisitionStatus)).Cast<RequisitionStatus>().Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name", status);

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", requisitions);
        }

        return View(requisitions);
    }

    [PermissionAuthorize(PermissionNames.CashRequisitionManage)]
    public async Task<IActionResult> Create()
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        await PopulateDropdownsAsync();
        return View(new CreateCashRequisitionDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashRequisitionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCashRequisitionDto dto)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        dto.Status = RequisitionStatus.Draft;
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateCashRequisitionCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.CashRequisitionManage)]
    public async Task<IActionResult> Edit(long id)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        var requisition = await _mediator.Send(new GetCashRequisitionByIdQuery(id));
        if (requisition is null)
        {
            return NotFound();
        }

        var dto = new UpdateCashRequisitionDto
        {
            Id = requisition.Id,
            RequisitionNumber = requisition.RequisitionNumber,
            RequestDate = requisition.RequestDate,
            RequiredByDate = requisition.RequiredByDate,
            Description = requisition.Description,
            Status = requisition.Status,
            RejectionReason = requisition.RejectionReason, RejectedBy = requisition.RejectedBy, RejectedAt = requisition.RejectedAt,
            RequesterEmployeeId = requisition.RequesterEmployeeId,
            PaymentMethod = requisition.PaymentMethod,
            DepartmentId = requisition.DepartmentId,
            ProjectId = requisition.ProjectId,
            Details = requisition.Details.Select(d => new CreateCashRequisitionDetailDto
            {
                MaterialId = d.MaterialId,
                Quantity = d.Quantity,
                UnitOfMeasure = d.UnitOfMeasure,
                EstimatedUnitPrice = d.EstimatedUnitPrice,
                Remarks = d.Remarks
            }).ToList()
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashRequisitionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateCashRequisitionDto dto)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        dto.Status = RequisitionStatus.Draft;
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var result = await _mediator.Send(new UpdateCashRequisitionCommand(dto));
        if (result == UpdateCashRequisitionResult.NotFound)
        {
            return NotFound();
        }

        if (result == UpdateCashRequisitionResult.Locked)
        {
            ModelState.AddModelError(string.Empty, "Only Draft requisitions can be edited.");
            await PopulateDropdownsAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashRequisitionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetCashRequisitionActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashRequisitionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WorkflowAction(long id, string workflowAction, string? rejectionReason)
    {
        var assignment = await GetCurrentAssignmentAsync();
        if (assignment is null) return Forbid();
        var requisition = await _db.CashRequisitions.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (requisition is null) return NotFound();
        var action = workflowAction?.Trim().ToLowerInvariant();
        var canPerform = action switch { "submit" => assignment.CanSubmit, "request" => assignment.CanRequestApproval, "approve" => assignment.CanApprove, "reject" => assignment.CanReject, "cancel" => assignment.CanCancel, _ => false };
        if (!canPerform) { TempData["ErrorMessage"] = "You are not assigned to perform that action."; return RedirectToAction(nameof(Index)); }
        if (action == "reject" && string.IsNullOrWhiteSpace(rejectionReason)) { TempData["ErrorMessage"] = "A rejection reason is required."; return RedirectToAction(nameof(Index)); }
        if (rejectionReason?.Length > 1000) { TempData["ErrorMessage"] = "The rejection reason cannot exceed 1,000 characters."; return RedirectToAction(nameof(Index)); }
        if (action == "reject" && requisition.Status == RequisitionStatus.Approved && await _db.CashDisbursements.AnyAsync(d => d.CashRequisitionId == id && d.IsActive)) { TempData["ErrorMessage"] = "This approved requisition has active disbursements and cannot be returned to Draft."; return RedirectToAction(nameof(Index)); }
        var transition = action switch
        {
            "submit" => (From: RequisitionStatus.Draft, To: RequisitionStatus.Submitted),
            "request" => (From: RequisitionStatus.Submitted, To: RequisitionStatus.AwaitingApproval),
            "approve" => (From: RequisitionStatus.AwaitingApproval, To: RequisitionStatus.Approved),
            "reject" when requisition.Status is RequisitionStatus.Submitted or RequisitionStatus.AwaitingApproval or RequisitionStatus.Approved => (From: requisition.Status, To: RequisitionStatus.Draft),
            "reject" => (From: RequisitionStatus.Draft, To: RequisitionStatus.Draft),
            "cancel" when requisition.Status is RequisitionStatus.Draft or RequisitionStatus.Submitted or RequisitionStatus.AwaitingApproval or RequisitionStatus.Rejected => (From: requisition.Status, To: RequisitionStatus.Cancelled),
            "cancel" => (From: RequisitionStatus.Draft, To: RequisitionStatus.Cancelled),
            _ => (From: requisition.Status, To: requisition.Status)
        };
        if (requisition.Status == transition.To) { TempData["StatusMessage"] = $"Cash Requisition {requisition.RequisitionNumber} is already {transition.To}."; return RedirectToAction(nameof(Index)); }
        if (requisition.Status != transition.From) { TempData["ErrorMessage"] = $"This action is no longer available because Cash Requisition {requisition.RequisitionNumber} is {requisition.Status}."; return RedirectToAction(nameof(Index)); }
        var updated = await _mediator.Send(new SetCashRequisitionStatusCommand(requisition.Id, transition.From, transition.To, User.Identity?.Name, action == "reject" ? rejectionReason : null));
        if (!updated) { TempData["ErrorMessage"] = $"Cash Requisition {requisition.RequisitionNumber} changed while this action was being processed. Please try again."; return RedirectToAction(nameof(Index)); }
        TempData["StatusMessage"] = action == "reject" ? $"Cash Requisition {requisition.RequisitionNumber} was returned to Draft for revision." : $"Cash Requisition {requisition.RequisitionNumber} is now {transition.To}.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Print(long id)
    {
        var data = await _mediator.Send(new GetCashRequisitionPrintDataQuery(id));
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
        var data = await _mediator.Send(new GetCashRequisitionPrintDataQuery(id));
        if (data is null)
        {
            return NotFound();
        }

        data.PrintedBy = User.Identity?.Name;
        data.PrintedAt = DateTime.Now;

        var pdfBytes = _pdfExporter.Export(data);
        return File(pdfBytes, "application/pdf", $"{data.RequisitionNumber}.pdf");
    }

    private async Task PopulateDropdownsAsync()
    {
        var employees = await _mediator.Send(new GetAllEmployeesQuery(PageSize: int.MaxValue));
        ViewBag.Employees = new SelectList(employees.Items, "Id", "EmployeeName");

        var departments = await _mediator.Send(new GetAllDepartmentsQuery(PageSize: int.MaxValue));
        ViewBag.Departments = new SelectList(departments.Items, "Id", "Name");

        var materials = await _mediator.Send(new GetAllMaterialsQuery(PageSize: int.MaxValue));
        // Format materials with Code - Name | Category for multicolumn dropdown
        var formattedMaterials = materials.Items.Select(m => new SelectListItem
        {
            Value = m.Id.ToString(),
            Text = $"{m.MaterialCode} | {m.Name} | {m.CategoryName}"
        }).ToList();
        ViewBag.Materials = formattedMaterials;

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }

    private async Task<CashRequisitionActionAssignment?> GetCurrentAssignmentAsync()
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idValue, out var userId) ? await _db.CashRequisitionActionAssignments.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == userId) : null;
    }
    private async Task<bool> HasActionAsync(Func<CashRequisitionActionAssignment, bool> predicate)
    {
        var assignment = await GetCurrentAssignmentAsync();
        return assignment is not null && predicate(assignment);
    }
}
