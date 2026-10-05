using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.PoBills;
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

[PermissionAuthorize(PermissionNames.PoBillView)]
public class PoBillsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<SavePoBillDto> _validator;
    private readonly AppDbContext _db;

    public PoBillsController(IMediator mediator, IValidator<SavePoBillDto> validator, AppDbContext db)
    {
        _mediator = mediator;
        _validator = validator;
        _db = db;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? billNumber = null, string? supplier = null, PoBillStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        ViewBag.ActionAssignment = await GetCurrentAssignmentAsync();
        var bills = await _mediator.Send(new GetAllPoBillsQuery(page, pageSize, billNumber, supplier, status, dateFrom, dateTo));

        ViewBag.BillNumber = billNumber;
        ViewBag.Supplier = supplier;
        ViewBag.Status = status;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;
        ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(PoBillStatus)).Cast<PoBillStatus>().Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name", status);

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", bills);
        }

        return View(bills);
    }

    [PermissionAuthorize(PermissionNames.PoBillManage)]
    public async Task<IActionResult> Create(long? purchaseOrderId = null)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        await PopulateAsync(purchaseOrderId, null);
        return View(new SavePoBillDto { PurchaseOrderId = purchaseOrderId ?? 0 });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PoBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SavePoBillDto dto)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        dto.Status = PoBillStatus.Draft;
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateAsync(dto.PurchaseOrderId, null, dto);
            return View(dto);
        }

        var result = await _mediator.Send(new CreatePoBillCommand(dto));
        if (result != PoBillBuildResult.Success)
        {
            ModelState.AddModelError(string.Empty, BuildErrorMessage(result));
            await PopulateAsync(dto.PurchaseOrderId, null, dto);
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.PoBillManage)]
    public async Task<IActionResult> Edit(long id)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        var bill = await _mediator.Send(new GetPoBillByIdQuery(id));
        if (bill is null)
        {
            return NotFound();
        }

        var dto = new SavePoBillDto
        {
            Id = bill.Id,
            BillNumber = bill.BillNumber,
            BillDate = bill.BillDate,
            DueDate = bill.DueDate,
            SupplierInvoiceNumber = bill.SupplierInvoiceNumber,
            Remarks = bill.Remarks,
            Status = bill.Status,
            PurchaseOrderId = bill.PurchaseOrderId,
            Details = bill.Details.Select(d => new PoBillLineInputDto
            {
                PurchaseOrderDetailId = d.PurchaseOrderDetailId,
                BilledQuantity = d.BilledQuantity
            }).ToList()
        };

        ViewBag.IsLocked = bill.Status != PoBillStatus.Draft;
        await PopulateAsync(bill.PurchaseOrderId, bill.Id, dto);
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PoBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SavePoBillDto dto)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        dto.Status = PoBillStatus.Draft;
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateAsync(dto.PurchaseOrderId, dto.Id, dto);
            return View(dto);
        }

        var result = await _mediator.Send(new UpdatePoBillCommand(dto));
        switch (result)
        {
            case UpdatePoBillResult.Success:
                return RedirectToAction(nameof(Index));
            case UpdatePoBillResult.NotFound:
                return NotFound();
        }

        ModelState.AddModelError(string.Empty, result switch
        {
            UpdatePoBillResult.Locked => "Only Draft bills can be edited.",
            UpdatePoBillResult.OverBilled => BuildErrorMessage(PoBillBuildResult.OverBilled),
            UpdatePoBillResult.PurchaseOrderNotBillable => BuildErrorMessage(PoBillBuildResult.PurchaseOrderNotBillable),
            _ => BuildErrorMessage(PoBillBuildResult.InvalidLine)
        });
        ViewBag.IsLocked = result == UpdatePoBillResult.Locked;
        await PopulateAsync(dto.PurchaseOrderId, dto.Id, dto);
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PoBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetPoBillActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PoBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WorkflowAction(long id, string workflowAction)
    {
        var assignment = await GetCurrentAssignmentAsync();
        if (assignment is null) return Forbid();
        var bill = await _db.PoBills.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (bill is null) return NotFound();
        var action = workflowAction?.Trim().ToLowerInvariant();
        var canPerform = action switch { "submit" => assignment.CanSubmit, "request" => assignment.CanRequestApproval, "approve" => assignment.CanApprove, "reject" => assignment.CanReject, "cancel" => assignment.CanCancel, _ => false };
        if (!canPerform) { TempData["ErrorMessage"] = "You are not assigned to perform that action."; return RedirectToAction(nameof(Index)); }
        var transition = action switch
        {
            "submit" => (From: PoBillStatus.Draft, To: PoBillStatus.Submitted),
            "request" => (From: PoBillStatus.Submitted, To: PoBillStatus.AwaitingApproval),
            "approve" => (From: PoBillStatus.AwaitingApproval, To: PoBillStatus.Approved),
            "reject" => (From: PoBillStatus.AwaitingApproval, To: PoBillStatus.Rejected),
            "cancel" when bill.Status is PoBillStatus.Draft or PoBillStatus.Submitted or PoBillStatus.AwaitingApproval or PoBillStatus.Rejected => (From: bill.Status, To: PoBillStatus.Cancelled),
            "cancel" => (From: PoBillStatus.Draft, To: PoBillStatus.Cancelled),
            _ => (From: bill.Status, To: bill.Status)
        };
        if (bill.Status == transition.To) { TempData["StatusMessage"] = $"PO Bill {bill.BillNumber} is already {transition.To}."; return RedirectToAction(nameof(Index)); }
        if (bill.Status != transition.From) { TempData["ErrorMessage"] = $"This action is no longer available because PO Bill {bill.BillNumber} is {bill.Status}."; return RedirectToAction(nameof(Index)); }
        var updated = await _mediator.Send(new SetPoBillStatusCommand(bill.Id, transition.From, transition.To, User.Identity?.Name));
        if (!updated) { TempData["ErrorMessage"] = $"PO Bill {bill.BillNumber} changed while this action was being processed. Please try again."; return RedirectToAction(nameof(Index)); }
        TempData["StatusMessage"] = $"PO Bill {bill.BillNumber} is now {transition.To}.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Print(long id)
    {
        var data = await _mediator.Send(new GetPoBillPrintDataQuery(id));
        if (data is null)
        {
            return NotFound();
        }

        data.PrintedBy = User.Identity?.Name;
        data.PrintedAt = DateTime.Now;

        return View(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetLines(long purchaseOrderId, long? excludeBillId = null)
    {
        var lines = await _mediator.Send(new GetPoBillableLinesQuery(purchaseOrderId, excludeBillId));
        return Json(lines.Select(l => new
        {
            purchaseOrderDetailId = l.PurchaseOrderDetailId,
            materialName = l.MaterialName,
            unitOfMeasure = l.UnitOfMeasure.ToString(),
            orderedQuantity = l.OrderedQuantity,
            alreadyBilledQuantity = l.AlreadyBilledQuantity,
            remainingQuantity = l.RemainingQuantity,
            unitPrice = l.UnitPrice,
            discountPercent = l.DiscountPercent,
            vatPercent = l.VatPercent,
            taxPercent = l.TaxPercent
        }));
    }

    private static string BuildErrorMessage(PoBillBuildResult result) => result switch
    {
        PoBillBuildResult.PurchaseOrderNotBillable => "The selected purchase order is not approved or is inactive, so it cannot be billed.",
        PoBillBuildResult.OverBilled => "One or more lines exceed the ordered quantity minus the quantity already billed.",
        _ => "One or more bill lines are invalid."
    };

    private async Task PopulateAsync(long? purchaseOrderId, long? billId, SavePoBillDto? dto = null)
    {
        var orders = await _mediator.Send(new GetBillablePurchaseOrdersQuery(purchaseOrderId));
        ViewBag.PurchaseOrders = orders.Select(o => new SelectListItem
        {
            Value = o.Id.ToString(),
            Text = $"{o.PONumber} | {o.SupplierName} | {o.ProjectName}",
            Selected = o.Id == purchaseOrderId
        }).ToList();
        ViewBag.BillId = billId;
        ViewBag.InitialQuantities = (dto?.Details ?? new())
            .GroupBy(d => d.PurchaseOrderDetailId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.BilledQuantity));
    }

    private async Task<PoBillActionAssignment?> GetCurrentAssignmentAsync()
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idValue, out var userId) ? await _db.PoBillActionAssignments.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == userId) : null;
    }
    private async Task<bool> HasActionAsync(Func<PoBillActionAssignment, bool> predicate)
    {
        var assignment = await GetCurrentAssignmentAsync();
        return assignment is not null && predicate(assignment);
    }
}
