using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.PurchaseOrders;
using BuilderERP.Application.Features.PurchaseOrders.Export;
using BuilderERP.Application.Features.VendorQuotations;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
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

[PermissionAuthorize(PermissionNames.PurchaseOrderView)]
public class PurchaseOrdersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreatePurchaseOrderDto> _createValidator;
    private readonly IValidator<UpdatePurchaseOrderDto> _updateValidator;
    private readonly PurchaseOrderPdfExporter _pdfExporter;
    private readonly AppDbContext _db;

    public PurchaseOrdersController(
        IMediator mediator,
        IValidator<CreatePurchaseOrderDto> createValidator,
        IValidator<UpdatePurchaseOrderDto> updateValidator,
        PurchaseOrderPdfExporter pdfExporter,
        AppDbContext db)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _pdfExporter = pdfExporter;
        _db = db;
    }

    public async Task<IActionResult> Index(
        int page = 1,
        int pageSize = 25,
        string? poNumber = null,
        DateTime? orderDateFrom = null,
        DateTime? orderDateTo = null,
        DateTime? deliveryDateFrom = null,
        DateTime? deliveryDateTo = null)
    {
        ViewBag.ActionAssignment = await GetCurrentAssignmentAsync();
        var orders = await _mediator.Send(new GetAllPurchaseOrdersQuery(
            page, pageSize, poNumber, orderDateFrom, orderDateTo, deliveryDateFrom, deliveryDateTo));

        ViewBag.PONumber = poNumber;
        ViewBag.OrderDateFrom = orderDateFrom;
        ViewBag.OrderDateTo = orderDateTo;
        ViewBag.DeliveryDateFrom = deliveryDateFrom;
        ViewBag.DeliveryDateTo = deliveryDateTo;

        var routeValues = new Dictionary<string, string?>();
        if (!string.IsNullOrWhiteSpace(poNumber)) routeValues["poNumber"] = poNumber;
        if (orderDateFrom.HasValue) routeValues["orderDateFrom"] = orderDateFrom.Value.ToString("yyyy-MM-dd");
        if (orderDateTo.HasValue) routeValues["orderDateTo"] = orderDateTo.Value.ToString("yyyy-MM-dd");
        if (deliveryDateFrom.HasValue) routeValues["deliveryDateFrom"] = deliveryDateFrom.Value.ToString("yyyy-MM-dd");
        if (deliveryDateTo.HasValue) routeValues["deliveryDateTo"] = deliveryDateTo.Value.ToString("yyyy-MM-dd");
        ViewBag.FilterRouteValues = routeValues;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", orders);
        }

        return View(orders);
    }

    [PermissionAuthorize(PermissionNames.PurchaseOrderManage)]
    public async Task<IActionResult> Create()
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        await PopulateDropdownsAsync();
        return View(new CreatePurchaseOrderDto());
    }

    [HttpGet]
    public async Task<IActionResult> Details(long id)
    {
        var order = await _mediator.Send(new GetPurchaseOrderByIdQuery(id));
        return order is null ? NotFound() : View(order);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PurchaseOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePurchaseOrderDto dto)
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

        await _mediator.Send(new CreatePurchaseOrderCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.PurchaseOrderManage)]
    public async Task<IActionResult> Edit(long id)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        ViewBag.ActionAssignment = await GetCurrentAssignmentAsync();
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
            RejectionReason = order.RejectionReason,
            RejectedBy = order.RejectedBy,
            RejectedAt = order.RejectedAt,
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
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        dto.Status = PurchaseOrderStatus.Draft;
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            ViewBag.ActionAssignment = await GetCurrentAssignmentAsync();
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
            ViewBag.ActionAssignment = await GetCurrentAssignmentAsync();
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

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PurchaseOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WorkflowAction(long id, string workflowAction, string? rejectionReason)
    {
        var assignment = await GetCurrentAssignmentAsync();
        if (assignment is null) return Forbid();

        var order = await _db.PurchaseOrders.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        if (order is null) return NotFound();

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

        if (action == "reject" && string.IsNullOrWhiteSpace(rejectionReason))
        {
            TempData["ErrorMessage"] = "A rejection reason is required before returning the purchase order for revision.";
            return RedirectToAction(nameof(Index));
        }
        if (rejectionReason?.Length > 1000)
        {
            TempData["ErrorMessage"] = "The rejection reason cannot exceed 1,000 characters.";
            return RedirectToAction(nameof(Index));
        }

        var transition = action switch
        {
            "submit" => (From: PurchaseOrderStatus.Draft, To: PurchaseOrderStatus.Submitted),
            "request" => (From: PurchaseOrderStatus.Submitted, To: PurchaseOrderStatus.AwaitingApproval),
            "approve" => (From: PurchaseOrderStatus.AwaitingApproval, To: PurchaseOrderStatus.Approved),
            "reject" when order.Status is PurchaseOrderStatus.Submitted
                or PurchaseOrderStatus.AwaitingApproval or PurchaseOrderStatus.Approved
                => (From: order.Status, To: PurchaseOrderStatus.Draft),
            "reject" => (From: PurchaseOrderStatus.Draft, To: PurchaseOrderStatus.Draft),
            "cancel" when order.Status is PurchaseOrderStatus.Draft
                or PurchaseOrderStatus.Submitted or PurchaseOrderStatus.AwaitingApproval or PurchaseOrderStatus.Rejected
                => (From: order.Status, To: PurchaseOrderStatus.Cancelled),
            "cancel" => (From: PurchaseOrderStatus.Draft, To: PurchaseOrderStatus.Cancelled),
            _ => (From: order.Status, To: order.Status)
        };

        // A repeated POST can arrive after the first request has already completed.
        // Treat that as success instead of showing a misleading permission error.
        if (order.Status == transition.To)
        {
            await _mediator.Send(new InvalidatePurchaseOrderCacheCommand());
            TempData["StatusMessage"] = $"Purchase Order {order.PONumber} is already {transition.To}.";
            return RedirectToAction(nameof(Index));
        }

        if (order.Status != transition.From)
        {
            TempData["ErrorMessage"] = $"This action is no longer available because Purchase Order {order.PONumber} is {order.Status}.";
            return RedirectToAction(nameof(Index));
        }

        var updated = await _mediator.Send(new SetPurchaseOrderStatusCommand(
            order.Id, transition.From, transition.To, User.Identity?.Name,
            action == "reject" ? rejectionReason : null));
        if (!updated)
        {
            TempData["ErrorMessage"] = $"Purchase Order {order.PONumber} changed while this action was being processed. Please try again.";
            return RedirectToAction(nameof(Index));
        }

        TempData["StatusMessage"] = action == "reject"
            ? $"Purchase Order {order.PONumber} was returned to Draft for revision."
            : $"Purchase Order {order.PONumber} is now {transition.To}.";
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
            Text = $"{q.RequisitionNumber} | {q.QuotationNumber} | {q.ProjectName}"
        }).ToList();

        var materials = await _mediator.Send(new GetAllMaterialsQuery(PageSize: int.MaxValue));
        // Format materials with Code - Name | Category for multicolumn dropdown
        var formattedMaterials = materials.Items.Select(m => new SelectListItem
        {
            Value = m.Id.ToString(),
            Text = $"{m.MaterialCode} | {m.Name} | {m.CategoryName}"
        }).ToList();
        ViewBag.Materials = formattedMaterials;

        var actionUsers = await (
            from action in _db.PurchaseOrderActionAssignments.AsNoTracking()
            join user in _db.Users.AsNoTracking() on action.UserId equals user.Id
            where user.IsActive && (action.CanSubmit || action.CanRequestApproval || action.CanApprove || action.CanReject)
            orderby user.FullName
            select new { user.FullName, action.CanSubmit, action.CanRequestApproval, action.CanApprove, action.CanReject })
            .ToListAsync();

        ViewBag.ApprovalAssignments = actionUsers.Select(x =>
        {
            var actions = new List<string>();
            if (x.CanSubmit) actions.Add("submits PO");
            if (x.CanRequestApproval) actions.Add("requests approval");
            if (x.CanApprove) actions.Add("approves");
            if (x.CanReject) actions.Add("rejects");
            var nameParts = x.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return new PurchaseOrderApprovalAssignmentViewModel
            {
                FullName = x.FullName,
                Actions = string.Join(" and ", actions),
                Initials = string.Concat(nameParts.Take(2).Select(p => char.ToUpperInvariant(p[0])))
            };
        }).ToList();
    }

    private async Task<PurchaseOrderActionAssignment?> GetCurrentAssignmentAsync()
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idValue, out var userId)
            ? await _db.PurchaseOrderActionAssignments.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == userId)
            : null;
    }

    private async Task<bool> HasActionAsync(Func<PurchaseOrderActionAssignment, bool> predicate)
    {
        var assignment = await GetCurrentAssignmentAsync();
        return assignment is not null && predicate(assignment);
    }
}
