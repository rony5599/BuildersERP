using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.CashPurchaseOrders;
using BuilderERP.Application.Features.CashPurchaseOrders.Export;
using BuilderERP.Application.Features.CashRequisitions;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.Suppliers;
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

[PermissionAuthorize(PermissionNames.CashPurchaseOrderView)]
public class CashPurchaseOrdersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateCashPurchaseOrderDto> _createValidator;
    private readonly IValidator<UpdateCashPurchaseOrderDto> _updateValidator;
    private readonly CashPurchaseOrderPdfExporter _pdfExporter;
    private readonly AppDbContext _db;

    public CashPurchaseOrdersController(
        IMediator mediator,
        IValidator<CreateCashPurchaseOrderDto> createValidator,
        IValidator<UpdateCashPurchaseOrderDto> updateValidator,
        CashPurchaseOrderPdfExporter pdfExporter, AppDbContext db)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _pdfExporter = pdfExporter;
        _db = db;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? cpoNumber = null, long? projectId = null, PurchaseOrderStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        ViewBag.ActionAssignment = await GetCurrentAssignmentAsync();
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
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        await PopulateDropdownsAsync();
        return View(new CreateCashPurchaseOrderDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashPurchaseOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCashPurchaseOrderDto dto)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        dto.Status = PurchaseOrderStatus.Draft;
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
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
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
            RejectionReason = order.RejectionReason, RejectedBy = order.RejectedBy, RejectedAt = order.RejectedAt,
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
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        dto.Status = PurchaseOrderStatus.Draft;
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
        if (requisition.Status != RequisitionStatus.Approved)
        {
            return BadRequest("Only approved Cash Requisitions can be used.");
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

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashPurchaseOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WorkflowAction(long id, string workflowAction, string? rejectionReason)
    {
        var assignment = await GetCurrentAssignmentAsync();
        if (assignment is null) return Forbid();
        var order = await _db.CashPurchaseOrders.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        if (order is null) return NotFound();
        var action = workflowAction?.Trim().ToLowerInvariant();
        var canPerform = action switch { "submit" => assignment.CanSubmit, "request" => assignment.CanRequestApproval, "approve" => assignment.CanApprove, "reject" => assignment.CanReject, "cancel" => assignment.CanCancel, _ => false };
        if (!canPerform) { TempData["ErrorMessage"] = "You are not assigned to perform that action."; return RedirectToAction(nameof(Index)); }
        if (action == "reject" && string.IsNullOrWhiteSpace(rejectionReason)) { TempData["ErrorMessage"] = "A rejection reason is required."; return RedirectToAction(nameof(Index)); }
        if (rejectionReason?.Length > 1000) { TempData["ErrorMessage"] = "The rejection reason cannot exceed 1,000 characters."; return RedirectToAction(nameof(Index)); }
        if (action == "reject" && order.Status == PurchaseOrderStatus.Approved && (order.ReceivedAmount > 0 || await _db.CashPoBills.AnyAsync(b => b.CashPurchaseOrderId == id && b.IsActive && b.Status != PoBillStatus.Cancelled))) { TempData["ErrorMessage"] = "This approved Cash PO has receipts or bills and cannot be returned to Draft."; return RedirectToAction(nameof(Index)); }
        var transition = action switch
        {
            "submit" => (From: PurchaseOrderStatus.Draft, To: PurchaseOrderStatus.Submitted),
            "request" => (From: PurchaseOrderStatus.Submitted, To: PurchaseOrderStatus.AwaitingApproval),
            "approve" => (From: PurchaseOrderStatus.AwaitingApproval, To: PurchaseOrderStatus.Approved),
            "reject" when order.Status is PurchaseOrderStatus.Submitted or PurchaseOrderStatus.AwaitingApproval or PurchaseOrderStatus.Approved => (From: order.Status, To: PurchaseOrderStatus.Draft),
            "reject" => (From: PurchaseOrderStatus.Draft, To: PurchaseOrderStatus.Draft),
            "cancel" when order.Status is PurchaseOrderStatus.Draft or PurchaseOrderStatus.Submitted or PurchaseOrderStatus.AwaitingApproval or PurchaseOrderStatus.Rejected => (From: order.Status, To: PurchaseOrderStatus.Cancelled),
            "cancel" => (From: PurchaseOrderStatus.Draft, To: PurchaseOrderStatus.Cancelled),
            _ => (From: order.Status, To: order.Status)
        };
        if (order.Status == transition.To) { TempData["StatusMessage"] = $"Cash Purchase Order {order.CPONumber} is already {transition.To}."; return RedirectToAction(nameof(Index)); }
        if (order.Status != transition.From) { TempData["ErrorMessage"] = $"This action is no longer available because Cash Purchase Order {order.CPONumber} is {order.Status}."; return RedirectToAction(nameof(Index)); }
        var updated = await _mediator.Send(new SetCashPurchaseOrderStatusCommand(order.Id, transition.From, transition.To, User.Identity?.Name, action == "reject" ? rejectionReason : null));
        if (!updated) { TempData["ErrorMessage"] = $"Cash Purchase Order {order.CPONumber} changed while this action was being processed. Please try again."; return RedirectToAction(nameof(Index)); }
        TempData["StatusMessage"] = action == "reject" ? $"Cash Purchase Order {order.CPONumber} was returned to Draft for revision." : $"Cash Purchase Order {order.CPONumber} is now {transition.To}.";
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
        var requisitions = await _mediator.Send(new GetAllCashRequisitionsQuery(PageSize: int.MaxValue, Status: RequisitionStatus.Approved));
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

    private async Task<CashPurchaseOrderActionAssignment?> GetCurrentAssignmentAsync()
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idValue, out var userId) ? await _db.CashPurchaseOrderActionAssignments.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == userId) : null;
    }
    private async Task<bool> HasActionAsync(Func<CashPurchaseOrderActionAssignment, bool> predicate)
    {
        var assignment = await GetCurrentAssignmentAsync();
        return assignment is not null && predicate(assignment);
    }
}
