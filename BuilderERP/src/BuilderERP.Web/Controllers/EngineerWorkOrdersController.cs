using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.EngineerWorkOrderRequisitions;
using BuilderERP.Application.Features.EngineerWorkOrders;
using BuilderERP.Application.Features.EngineerWorkOrders.Export;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.Suppliers;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Entities;
using BuilderERP.Infrastructure.Persistence;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using BuilderERP.Web.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.EngineerWorkOrderView)]
public class EngineerWorkOrdersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateEngineerWorkOrderDto> _createValidator;
    private readonly IValidator<UpdateEngineerWorkOrderDto> _updateValidator;
    private readonly IValidator<SaveEngineerWorkOrderPaymentHeadsDto> _paymentHeadsValidator;
    private readonly EngineerWorkOrderPdfExporter _pdfExporter;
    private readonly AppDbContext _db;

    public EngineerWorkOrdersController(
        IMediator mediator,
        IValidator<CreateEngineerWorkOrderDto> createValidator,
        IValidator<UpdateEngineerWorkOrderDto> updateValidator,
        IValidator<SaveEngineerWorkOrderPaymentHeadsDto> paymentHeadsValidator,
        EngineerWorkOrderPdfExporter pdfExporter,
        AppDbContext db)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _paymentHeadsValidator = paymentHeadsValidator;
        _pdfExporter = pdfExporter;
        _db = db;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? workOrderNo = null, long? projectId = null, EngineerWorkOrderStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        ViewBag.ActionAssignment = await GetCurrentAssignmentAsync();
        var workOrders = await _mediator.Send(new GetAllEngineerWorkOrdersQuery(page, pageSize, workOrderNo, projectId, status, dateFrom, dateTo));

        ViewBag.WorkOrderNo = workOrderNo;
        ViewBag.ProjectId = projectId;
        ViewBag.Status = status;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);
        ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(EngineerWorkOrderStatus)).Cast<EngineerWorkOrderStatus>().Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name", status);

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", workOrders);
        }

        return View(workOrders);
    }

    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    public async Task<IActionResult> Create()
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        await PopulateDropdownsAsync();
        return View(new CreateEngineerWorkOrderDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEngineerWorkOrderDto dto)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        dto.Status = EngineerWorkOrderStatus.Draft;
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateEngineerWorkOrderCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    public async Task<IActionResult> Edit(long id)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        var workOrder = await _mediator.Send(new GetEngineerWorkOrderByIdQuery(id));
        if (workOrder is null)
        {
            return NotFound();
        }

        var dto = new UpdateEngineerWorkOrderDto
        {
            Id = workOrder.Id,
            WorkOrderNo = workOrder.WorkOrderNo,
            EngineerWorkOrderRequisitionId = workOrder.EngineerWorkOrderRequisitionId,
            SupplierId = workOrder.SupplierId,
            TermsAndCondition = workOrder.TermsAndCondition,
            Status = workOrder.Status,
            Details = workOrder.Details.Select(d => new CreateEngineerWorkOrderDetailDto
            {
                MaterialId = d.MaterialId,
                UnitOfMeasure = d.UnitOfMeasure,
                Qty = d.Qty,
                Rate = d.Rate,
                Remarks = d.Remarks
            }).ToList(),
            PaymentHeads = workOrder.PaymentHeads
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateEngineerWorkOrderDto dto)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        dto.Status = EngineerWorkOrderStatus.Draft;
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var result = await _mediator.Send(new UpdateEngineerWorkOrderCommand(dto));
        if (result == UpdateEngineerWorkOrderResult.NotFound)
        {
            return NotFound();
        }

        if (result == UpdateEngineerWorkOrderResult.Locked)
        {
            ModelState.AddModelError(string.Empty, "This work order is no longer editable in its current status.");
            await PopulateDropdownsAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    public async Task<IActionResult> Revise(long id)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        var workOrder = await _mediator.Send(new GetEngineerWorkOrderByIdQuery(id));
        if (workOrder is null)
        {
            return NotFound();
        }

        if (!workOrder.IsLatestRevision)
        {
            TempData["Error"] = "Only the latest revision of a work order can be revised.";
            return RedirectToAction(nameof(Index));
        }

        var dto = new CreateEngineerWorkOrderDto
        {
            EngineerWorkOrderRequisitionId = workOrder.EngineerWorkOrderRequisitionId,
            SupplierId = workOrder.SupplierId,
            TermsAndCondition = workOrder.TermsAndCondition,
            Status = Domain.Enums.EngineerWorkOrderStatus.Draft,
            Details = workOrder.Details.Select(d => new CreateEngineerWorkOrderDetailDto
            {
                MaterialId = d.MaterialId,
                UnitOfMeasure = d.UnitOfMeasure,
                Qty = d.Qty,
                Rate = d.Rate,
                Remarks = d.Remarks
            }).ToList(),
            PaymentHeads = workOrder.PaymentHeads
        };

        await PopulateDropdownsAsync();
        ViewBag.PreviousWorkOrderId = id;
        ViewBag.IsRevision = true;
        return View("Create", dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Revise(long previousWorkOrderId, CreateEngineerWorkOrderDto dto)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        dto.Status = EngineerWorkOrderStatus.Draft;
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            ViewBag.PreviousWorkOrderId = previousWorkOrderId;
            ViewBag.IsRevision = true;
            return View("Create", dto);
        }

        var result = await _mediator.Send(new CreateEngineerWorkOrderRevisionCommand(previousWorkOrderId, dto));

        if (result == CreateEngineerWorkOrderRevisionResult.NotFound)
        {
            return NotFound();
        }

        if (result == CreateEngineerWorkOrderRevisionResult.NotLatestRevision)
        {
            ModelState.AddModelError(string.Empty, "Only the latest revision of a work order can be revised.");
            await PopulateDropdownsAsync();
            ViewBag.PreviousWorkOrderId = previousWorkOrderId;
            ViewBag.IsRevision = true;
            return View("Create", dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WorkflowAction(long id, string workflowAction)
    {
        var assignment = await GetCurrentAssignmentAsync();
        if (assignment is null) return Forbid();

        var workOrder = await _db.EngineerWorkOrders.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        if (workOrder is null) return NotFound();

        var action = workflowAction?.Trim().ToLowerInvariant();
        var canPerform = action switch
        {
            "submit" => assignment.CanSubmit,
            "request" => assignment.CanRequestApproval,
            "approve" => assignment.CanApprove,
            "reject" => assignment.CanReject,
            "cancel" => assignment.CanCancel,
            _ => false
        };

        if (!canPerform)
        {
            TempData["ErrorMessage"] = "You are not assigned to perform that action.";
            return RedirectToAction(nameof(Index));
        }

        if (!workOrder.IsLatestRevision)
        {
            TempData["ErrorMessage"] = $"Work Order {workOrder.WorkOrderNo} is not the latest revision.";
            return RedirectToAction(nameof(Index));
        }

        var transition = action switch
        {
            "submit" => (From: EngineerWorkOrderStatus.Draft, To: EngineerWorkOrderStatus.Submitted),
            "request" => (From: EngineerWorkOrderStatus.Submitted, To: EngineerWorkOrderStatus.UnderApproval),
            "approve" => (From: EngineerWorkOrderStatus.UnderApproval, To: EngineerWorkOrderStatus.Approved),
            "reject" => (From: EngineerWorkOrderStatus.UnderApproval, To: EngineerWorkOrderStatus.Rejected),
            "cancel" when workOrder.Status is EngineerWorkOrderStatus.Draft
                or EngineerWorkOrderStatus.Submitted or EngineerWorkOrderStatus.UnderApproval or EngineerWorkOrderStatus.Rejected
                => (From: workOrder.Status, To: EngineerWorkOrderStatus.Cancelled),
            "cancel" => (From: EngineerWorkOrderStatus.Draft, To: EngineerWorkOrderStatus.Cancelled),
            _ => (From: workOrder.Status, To: workOrder.Status)
        };

        if (workOrder.Status == transition.To)
        {
            TempData["StatusMessage"] = $"Work Order {workOrder.WorkOrderNo} is already {transition.To}.";
            return RedirectToAction(nameof(Index));
        }

        if (workOrder.Status != transition.From)
        {
            TempData["ErrorMessage"] = $"This action is no longer available because Work Order {workOrder.WorkOrderNo} is {workOrder.Status}.";
            return RedirectToAction(nameof(Index));
        }

        var updated = await _mediator.Send(new SetEngineerWorkOrderStatusCommand(
            workOrder.Id, transition.From, transition.To, User.Identity?.Name));
        if (!updated)
        {
            TempData["ErrorMessage"] = $"Work Order {workOrder.WorkOrderNo} changed while this action was being processed. Please try again.";
            return RedirectToAction(nameof(Index));
        }

        TempData["StatusMessage"] = $"Work Order {workOrder.WorkOrderNo} is now {transition.To}.";
        return RedirectToAction(nameof(Index));
    }

    // Payment heads of an approved work order (whose form is locked) can be set until its first bill.
    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    public async Task<IActionResult> PaymentHeads(long id)
    {
        var workOrder = await _mediator.Send(new GetEngineerWorkOrderByIdQuery(id));
        if (workOrder is null)
        {
            return NotFound();
        }

        ViewBag.WorkOrder = workOrder;
        return View(new SaveEngineerWorkOrderPaymentHeadsDto { EngineerWorkOrderId = id, PaymentHeads = workOrder.PaymentHeads });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PaymentHeads(SaveEngineerWorkOrderPaymentHeadsDto dto)
    {
        var workOrder = await _mediator.Send(new GetEngineerWorkOrderByIdQuery(dto.EngineerWorkOrderId));
        if (workOrder is null)
        {
            return NotFound();
        }

        var validationResult = await _paymentHeadsValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            ViewBag.WorkOrder = workOrder;
            return View(dto);
        }

        var result = await _mediator.Send(new SetEngineerWorkOrderPaymentHeadsCommand(dto));
        if (result != SetEngineerWorkOrderPaymentHeadsResult.Success)
        {
            ModelState.AddModelError(string.Empty, result switch
            {
                SetEngineerWorkOrderPaymentHeadsResult.NotLatestRevision => "Only the latest revision of a work order can be changed.",
                SetEngineerWorkOrderPaymentHeadsResult.AlreadyBilled => "This work order already has bills, so its payment heads are fixed. Create a revision to change them.",
                _ => "The work order was not found."
            });
            ViewBag.WorkOrder = workOrder;
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(long id)
    {
        var workOrder = await _mediator.Send(new GetEngineerWorkOrderByIdQuery(id));
        if (workOrder is null)
        {
            return NotFound();
        }

        return PartialView("_DetailsModal", workOrder);
    }

    public async Task<IActionResult> Print(long id)
    {
        var data = await _mediator.Send(new GetEngineerWorkOrderPrintDataQuery(id));
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
        var data = await _mediator.Send(new GetEngineerWorkOrderPrintDataQuery(id));
        if (data is null)
        {
            return NotFound();
        }

        data.PrintedBy = User.Identity?.Name;
        data.PrintedAt = DateTime.Now;

        var pdfBytes = _pdfExporter.Export(data);
        return File(pdfBytes, "application/pdf", $"{data.WorkOrderNo}.pdf");
    }

    [HttpGet]
    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    public async Task<IActionResult> RequisitionItems(long requisitionId)
    {
        var requisition = await _mediator.Send(new GetEngineerWorkOrderRequisitionByIdQuery(requisitionId));
        if (requisition is null)
        {
            return NotFound();
        }

        var items = requisition.Details.Select(d => new
        {
            materialId = d.MaterialId,
            unitOfMeasure = (int)d.UnitOfMeasure,
            qty = d.Quantity,
            rate = d.EstimatedUnitPrice,
            remarks = d.Remarks
        });

        return Json(items);
    }

    private async Task PopulateDropdownsAsync()
    {
        var requisitions = await _mediator.Send(new GetAllEngineerWorkOrderRequisitionsQuery(PageSize: int.MaxValue));
        ViewBag.EngineerWorkOrderRequisitions = requisitions.Items.Select(r => new SelectListItem
        {
            Value = r.Id.ToString(),
            Text = $"{r.RequisitionNumber} | {r.ProjectName}"
        }).ToList();

        var suppliers = await _mediator.Send(new GetAllSuppliersQuery(PageSize: int.MaxValue));
        ViewBag.Suppliers = new SelectList(suppliers.Items, "Id", "Name");

        var materials = await _mediator.Send(new GetAllMaterialsQuery(PageSize: int.MaxValue));
        // Format materials with Code | Name | Category for multicolumn dropdown
        ViewBag.Materials = materials.Items.Select(m => new SelectListItem
        {
            Value = m.Id.ToString(),
            Text = $"{m.MaterialCode} | {m.Name} | {m.CategoryName}"
        }).ToList();

        var actionUsers = await (
            from action in _db.EngineerWorkOrderActionAssignments.AsNoTracking()
            join user in _db.Users.AsNoTracking() on action.UserId equals user.Id
            where user.IsActive && (action.CanSubmit || action.CanRequestApproval || action.CanApprove || action.CanReject)
            orderby user.FullName
            select new { user.FullName, action.CanSubmit, action.CanRequestApproval, action.CanApprove, action.CanReject })
            .ToListAsync();

        ViewBag.ApprovalAssignments = actionUsers.Select(x =>
        {
            var actions = new List<string>();
            if (x.CanSubmit) actions.Add("submits EWO");
            if (x.CanRequestApproval) actions.Add("requests approval");
            if (x.CanApprove) actions.Add("approves");
            if (x.CanReject) actions.Add("rejects");
            var nameParts = x.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return new EngineerWorkOrderApprovalAssignmentViewModel
            {
                FullName = x.FullName,
                Actions = string.Join(" and ", actions),
                Initials = string.Concat(nameParts.Take(2).Select(p => char.ToUpperInvariant(p[0])))
            };
        }).ToList();
    }

    private async Task<EngineerWorkOrderActionAssignment?> GetCurrentAssignmentAsync()
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idValue, out var userId)
            ? await _db.EngineerWorkOrderActionAssignments.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == userId)
            : null;
    }

    private async Task<bool> HasActionAsync(Func<EngineerWorkOrderActionAssignment, bool> predicate)
    {
        var assignment = await GetCurrentAssignmentAsync();
        return assignment is not null && predicate(assignment);
    }
}
