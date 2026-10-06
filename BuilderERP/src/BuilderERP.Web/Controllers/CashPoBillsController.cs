using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.CashPoBills;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Entities;
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

[PermissionAuthorize(PermissionNames.CashPoBillView)]
public class CashPoBillsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<SaveCashPoBillDto> _validator;
    private readonly AppDbContext _db;

    public CashPoBillsController(IMediator mediator, IValidator<SaveCashPoBillDto> validator, AppDbContext db)
    {
        _mediator = mediator;
        _validator = validator;
        _db = db;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? billNumber = null, string? requester = null, PoBillStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        ViewBag.ActionAssignment = await GetCurrentAssignmentAsync();
        var bills = await _mediator.Send(new GetAllCashPoBillsQuery(page, pageSize, billNumber, requester, status, dateFrom, dateTo));

        ViewBag.BillNumber = billNumber;
        ViewBag.Requester = requester;
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

    [PermissionAuthorize(PermissionNames.CashPoBillManage)]
    public async Task<IActionResult> Create(long? cashPurchaseOrderId = null)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        await PopulateAsync(cashPurchaseOrderId, null);
        return View(new SaveCashPoBillDto { CashPurchaseOrderId = cashPurchaseOrderId ?? 0 });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashPoBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveCashPoBillDto dto)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        dto.Status = PoBillStatus.Draft;
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateAsync(dto.CashPurchaseOrderId, null, dto);
            return View(dto);
        }

        var result = await _mediator.Send(new CreateCashPoBillCommand(dto));
        if (result != CashPoBillBuildResult.Success)
        {
            ModelState.AddModelError(string.Empty, BuildErrorMessage(result));
            await PopulateAsync(dto.CashPurchaseOrderId, null, dto);
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.CashPoBillManage)]
    public async Task<IActionResult> Edit(long id)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        var bill = await _mediator.Send(new GetCashPoBillByIdQuery(id));
        if (bill is null)
        {
            return NotFound();
        }

        var dto = new SaveCashPoBillDto
        {
            Id = bill.Id,
            BillNumber = bill.BillNumber,
            BillDate = bill.BillDate,
            MemoNumber = bill.MemoNumber,
            Remarks = bill.Remarks,
            Status = bill.Status,
            RejectionReason = bill.RejectionReason, RejectedBy = bill.RejectedBy, RejectedAt = bill.RejectedAt,
            CashPurchaseOrderId = bill.CashPurchaseOrderId,
            Details = bill.Details.Select(d => new CashPoBillLineInputDto
            {
                CashPurchaseOrderDetailId = d.CashPurchaseOrderDetailId,
                BilledQuantity = d.BilledQuantity
            }).ToList()
        };

        ViewBag.IsLocked = bill.Status != PoBillStatus.Draft;
        await PopulateAsync(bill.CashPurchaseOrderId, bill.Id, dto);
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashPoBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SaveCashPoBillDto dto)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        dto.Status = PoBillStatus.Draft;
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateAsync(dto.CashPurchaseOrderId, dto.Id, dto);
            return View(dto);
        }

        var result = await _mediator.Send(new UpdateCashPoBillCommand(dto));
        switch (result)
        {
            case UpdateCashPoBillResult.Success:
                return RedirectToAction(nameof(Index));
            case UpdateCashPoBillResult.NotFound:
                return NotFound();
        }

        ModelState.AddModelError(string.Empty, result switch
        {
            UpdateCashPoBillResult.Locked => "This bill is already approved or cancelled and cannot be edited.",
            UpdateCashPoBillResult.OverBilled => BuildErrorMessage(CashPoBillBuildResult.OverBilled),
            UpdateCashPoBillResult.OrderNotBillable => BuildErrorMessage(CashPoBillBuildResult.OrderNotBillable),
            _ => BuildErrorMessage(CashPoBillBuildResult.InvalidLine)
        });
        ViewBag.IsLocked = result == UpdateCashPoBillResult.Locked;
        await PopulateAsync(dto.CashPurchaseOrderId, dto.Id, dto);
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashPoBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetCashPoBillActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, PermissionAuthorize(PermissionNames.CashPoBillManage), ValidateAntiForgeryToken]
    public async Task<IActionResult> WorkflowAction(long id, string workflowAction, string? rejectionReason)
    {
        var assignment = await GetCurrentAssignmentAsync();
        if (assignment is null) return Forbid();
        var bill = await _db.CashPoBills.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (bill is null) return NotFound();
        var action = workflowAction?.Trim().ToLowerInvariant();
        var permitted = action switch { "submit" => assignment.CanSubmit, "request" => assignment.CanRequestApproval, "approve" => assignment.CanApprove, "reject" => assignment.CanReject, "cancel" => assignment.CanCancel, _ => false };
        if (!permitted) { TempData["ErrorMessage"] = "You are not assigned to perform that action."; return RedirectToAction(nameof(Index)); }
        if (action == "reject" && string.IsNullOrWhiteSpace(rejectionReason)) { TempData["ErrorMessage"] = "A rejection reason is required."; return RedirectToAction(nameof(Index)); }
        if (rejectionReason?.Length > 1000) { TempData["ErrorMessage"] = "The rejection reason cannot exceed 1,000 characters."; return RedirectToAction(nameof(Index)); }
        var transition = action switch
        {
            "submit" => (PoBillStatus.Draft, PoBillStatus.Submitted),
            "request" => (PoBillStatus.Submitted, PoBillStatus.AwaitingApproval),
            "approve" => (PoBillStatus.AwaitingApproval, PoBillStatus.Approved),
            "reject" when bill.Status is PoBillStatus.Submitted or PoBillStatus.AwaitingApproval or PoBillStatus.Approved => (bill.Status, PoBillStatus.Draft),
            "reject" => (PoBillStatus.Draft, PoBillStatus.Draft),
            "cancel" when bill.Status is PoBillStatus.Draft or PoBillStatus.Submitted or PoBillStatus.AwaitingApproval or PoBillStatus.Rejected => (bill.Status, PoBillStatus.Cancelled),
            _ => (bill.Status, bill.Status)
        };
        if (transition.Item1 == transition.Item2 || bill.Status != transition.Item1) { TempData["ErrorMessage"] = $"This action is not available because Cash PO Bill {bill.BillNumber} is {bill.Status}."; return RedirectToAction(nameof(Index)); }
        if (!await _mediator.Send(new SetCashPoBillStatusCommand(id, transition.Item1, transition.Item2, User.Identity?.Name, action == "reject" ? rejectionReason : null))) { TempData["ErrorMessage"] = "The bill changed while this action was processed. Please try again."; return RedirectToAction(nameof(Index)); }
        TempData["StatusMessage"] = action == "reject" ? $"Cash PO Bill {bill.BillNumber} was returned to Draft for revision." : $"Cash PO Bill {bill.BillNumber} is now {transition.Item2}.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Print(long id)
    {
        var data = await _mediator.Send(new GetCashPoBillPrintDataQuery(id));
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
        var lines = await _mediator.Send(new GetCashPoBillableLinesQuery(purchaseOrderId, excludeBillId));
        return Json(lines.Select(l => new
        {
            purchaseOrderDetailId = l.CashPurchaseOrderDetailId,
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

    private static string BuildErrorMessage(CashPoBillBuildResult result) => result switch
    {
        CashPoBillBuildResult.OrderNotBillable => "The selected cash purchase order is not approved or is inactive, so it cannot be billed.",
        CashPoBillBuildResult.OverBilled => "One or more lines exceed the ordered quantity minus the quantity already billed.",
        _ => "One or more bill lines are invalid."
    };

    private async Task PopulateAsync(long? cashPurchaseOrderId, long? billId, SaveCashPoBillDto? dto = null)
    {
        var orders = await _mediator.Send(new GetBillableCashPurchaseOrdersQuery(cashPurchaseOrderId));
        ViewBag.PurchaseOrders = orders.Select(o => new SelectListItem
        {
            Value = o.Id.ToString(),
            Text = $"{o.CPONumber} | {o.RequesterName} | {o.SupplierName}",
            Selected = o.Id == cashPurchaseOrderId
        }).ToList();
        ViewBag.BillId = billId;
        ViewBag.InitialQuantities = (dto?.Details ?? new())
            .GroupBy(d => d.CashPurchaseOrderDetailId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.BilledQuantity));
    }

    private async Task<CashPoBillActionAssignment?> GetCurrentAssignmentAsync()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var userId) ? await _db.CashPoBillActionAssignments.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == userId) : null;
    }
    private async Task<bool> HasActionAsync(Func<CashPoBillActionAssignment, bool> predicate)
    {
        var assignment = await GetCurrentAssignmentAsync(); return assignment is not null && predicate(assignment);
    }
}
