using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.PoBills;
using BuilderERP.Domain.Enums;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.PoBillView)]
public class PoBillsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<SavePoBillDto> _validator;

    public PoBillsController(IMediator mediator, IValidator<SavePoBillDto> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? billNumber = null, string? supplier = null, PoBillStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
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
        await PopulateAsync(purchaseOrderId, null);
        return View(new SavePoBillDto { PurchaseOrderId = purchaseOrderId ?? 0 });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PoBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SavePoBillDto dto)
    {
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
            UpdatePoBillResult.Locked => "This bill is already approved or cancelled and cannot be edited.",
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
}
