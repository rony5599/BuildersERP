using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.EwoBills;
using BuilderERP.Domain.Enums;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.EwoBillView)]
public class EwoBillsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<SaveEwoBillDto> _validator;

    public EwoBillsController(IMediator mediator, IValidator<SaveEwoBillDto> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? billNumber = null, string? supplier = null, PoBillStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var bills = await _mediator.Send(new GetAllEwoBillsQuery(page, pageSize, billNumber, supplier, status, dateFrom, dateTo));

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

    [PermissionAuthorize(PermissionNames.EwoBillManage)]
    public async Task<IActionResult> Create(long? engineerWorkOrderId = null)
    {
        var dto = new SaveEwoBillDto { EngineerWorkOrderId = engineerWorkOrderId ?? 0 };
        await PopulateAsync(dto);
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EwoBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveEwoBillDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateAsync(dto);
            return View(dto);
        }

        var result = await _mediator.Send(new CreateEwoBillCommand(dto));
        if (result != EwoBillBuildResult.Success)
        {
            ModelState.AddModelError(string.Empty, BuildErrorMessage(result));
            await PopulateAsync(dto);
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.EwoBillManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var bill = await _mediator.Send(new GetEwoBillByIdQuery(id));
        if (bill is null)
        {
            return NotFound();
        }

        // Approved/cancelled bills are shown as stored; a live form would recalculate against later bills.
        if (bill.Status != PoBillStatus.Draft)
        {
            return RedirectToAction(nameof(Print), new { id });
        }

        var dto = new SaveEwoBillDto
        {
            Id = bill.Id,
            BillNumber = bill.BillNumber,
            BillDate = bill.BillDate,
            ContractorBillNumber = bill.ContractorBillNumber,
            MrrNumber = bill.MrrNumber,
            Remarks = bill.Remarks,
            Status = bill.Status,
            EngineerWorkOrderId = bill.EngineerWorkOrderId,
            Details = bill.Details.Select(d => new EwoBillMeasurementInputDto
            {
                EngineerWorkOrderDetailId = d.EngineerWorkOrderDetailId,
                MeasuredQuantity = d.MeasuredQuantity
            }).ToList(),
            Heads = bill.Heads.Select(h => new EwoBillHeadInputDto
            {
                EngineerWorkOrderPaymentHeadId = h.EngineerWorkOrderPaymentHeadId,
                ClaimPercent = h.ClaimPercent
            }).ToList(),
            Adjustments = bill.Adjustments
        };

        await PopulateAsync(dto);
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EwoBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SaveEwoBillDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateAsync(dto);
            return View(dto);
        }

        var (result, buildResult) = await _mediator.Send(new UpdateEwoBillCommand(dto));
        switch (result)
        {
            case UpdateEwoBillResult.Success:
                return RedirectToAction(nameof(Index));
            case UpdateEwoBillResult.NotFound:
                return NotFound();
        }

        ModelState.AddModelError(string.Empty, result == UpdateEwoBillResult.Locked
            ? "This bill is already approved or cancelled and cannot be edited."
            : BuildErrorMessage(buildResult));
        ViewBag.IsLocked = result == UpdateEwoBillResult.Locked;
        await PopulateAsync(dto);
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EwoBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        var result = await _mediator.Send(new SetEwoBillActiveCommand(id, !isActive));
        var error = result switch
        {
            SetEwoBillActiveResult.LaterBillsExist => "A later bill of the same work order exists. Bills build on each other, so only the latest bill can be deactivated or restored.",
            SetEwoBillActiveResult.HasPayments => "This bill has active payments. Void its payments before deactivating it.",
            SetEwoBillActiveResult.StaleSnapshot => "An earlier bill of this work order changed since this bill was made, so its amounts are out of date. Raise a new bill instead of restoring this one.",
            _ => null
        };
        if (error is not null)
        {
            TempData["Error"] = error;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Print(long id)
    {
        var data = await _mediator.Send(new GetEwoBillPrintDataQuery(id));
        if (data is null)
        {
            return NotFound();
        }

        data.PrintedBy = User.Identity?.Name;
        data.PrintedAt = DateTime.Now;

        return View(data);
    }

    public async Task<IActionResult> Statement(long engineerWorkOrderId)
    {
        var data = await _mediator.Send(new GetEwoStatementQuery(engineerWorkOrderId));
        if (data is null)
        {
            return NotFound();
        }

        data.PrintedBy = User.Identity?.Name;
        data.PrintedAt = DateTime.Now;

        return View(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetBillableData(long engineerWorkOrderId, long? excludeBillId = null)
    {
        var data = await _mediator.Send(new GetEwoBillableDataQuery(engineerWorkOrderId, excludeBillId));
        if (data is null)
        {
            return NotFound();
        }

        return Json(new
        {
            data.WorkOrderNo,
            data.SupplierName,
            data.ContractAmount,
            data.PreviousCumulativePercent,
            data.PreviouslyCertified,
            data.PreviousBillCount,
            data.PendingDraftBillNumber,
            lines = data.Lines.Select(l => new
            {
                l.EngineerWorkOrderDetailId,
                l.MaterialName,
                unitOfMeasure = l.UnitOfMeasure.ToString(),
                l.OrderedQuantity,
                l.Rate,
                l.PreviousMeasuredQuantity
            }),
            heads = data.Heads.Select(h => new
            {
                h.EngineerWorkOrderPaymentHeadId,
                h.HeadName,
                h.Percent,
                h.ClaimedPercent,
                h.RemainingPercent
            })
        });
    }

    private static string BuildErrorMessage(EwoBillBuildResult result) => result switch
    {
        EwoBillBuildResult.OrderNotBillable => "The work order is not approved/active, or it has been revised. Bill the latest approved revision.",
        EwoBillBuildResult.NoPaymentHeads => "The work order has no payment heads. Set its payment heads (Engineer Work Orders > Payment Heads) before billing.",
        EwoBillBuildResult.PendingDraft => "Another bill of this work order is still Draft. Approve or cancel it before raising a new bill.",
        EwoBillBuildResult.OverMeasured => "A measured quantity is more than the work order quantity. Revise the work order if the work exceeds it.",
        EwoBillBuildResult.OverClaimed => "A claimed percent is more than what is left of that payment head.",
        EwoBillBuildResult.NothingPayable => "The net payable of this bill is zero or negative. Claim a payment head, increase the measurement or adjust the deductions.",
        _ => "One or more bill lines are invalid. Enter the measurement of at least one line."
    };

    private async Task PopulateAsync(SaveEwoBillDto dto)
    {
        var isEdit = dto.Id > 0;
        var orders = await _mediator.Send(new GetBillableEngineerWorkOrdersQuery(isEdit ? dto.EngineerWorkOrderId : null));
        ViewBag.WorkOrders = orders.Select(o => new SelectListItem
        {
            Value = o.Id.ToString(),
            Text = $"{o.WorkOrderNo} | {o.SupplierName} | {o.ProjectName}",
            Selected = o.Id == dto.EngineerWorkOrderId
        }).ToList();
        ViewBag.InitialQuantities = dto.Details
            .GroupBy(d => d.EngineerWorkOrderDetailId)
            .ToDictionary(g => g.Key.ToString(), g => g.Sum(x => x.MeasuredQuantity));
        ViewBag.InitialClaims = dto.Heads
            .GroupBy(h => h.EngineerWorkOrderPaymentHeadId)
            .ToDictionary(g => g.Key.ToString(), g => g.Sum(x => x.ClaimPercent));
    }
}
