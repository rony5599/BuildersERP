using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.SupplierPayments;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BuilderERP.Application.Features.Accounting;
using BuilderERP.Domain.Entities;
using BuilderERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SupplierPaymentView)]
public class SupplierPaymentsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateSupplierPaymentDto> _validator;
    private readonly IAccountingService _accounting;
    private readonly AppDbContext _db;

    public SupplierPaymentsController(IMediator mediator, IValidator<CreateSupplierPaymentDto> validator, IAccountingService accounting, AppDbContext db)
    {
        _mediator = mediator;
        _validator = validator;
        _accounting = accounting;
        _db = db;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? search = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var payments = await _mediator.Send(new GetAllSupplierPaymentsQuery(page, pageSize, search, dateFrom, dateTo));
        ViewBag.CanPostAccounting = await CanPostAccountingAsync();
        var pageIds = payments.Items.Select(p => p.Id).ToList();
        ViewBag.AccountingPostedIds = (await _db.JournalVouchers.AsNoTracking()
            .Where(v => v.SourceId.HasValue && pageIds.Contains(v.SourceId.Value)
                && (v.SourceType == "SupplierPayment" || v.SourceType.StartsWith("SupplierPayment.Repost."))
                && v.Status == JournalVoucherStatus.Posted && !v.IsDeleted)
            .Select(v => v.SourceId!.Value).ToListAsync()).ToHashSet();

        ViewBag.Search = search;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", payments);
        }

        return View(payments);
    }

    [PermissionAuthorize(PermissionNames.SupplierPaymentManage)]
    public async Task<IActionResult> Create(long? poBillId = null, long? ewoBillId = null)
    {
        await PopulateAsync();
        return View(new CreateSupplierPaymentDto { PoBillId = poBillId, EwoBillId = ewoBillId });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SupplierPaymentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSupplierPaymentDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateAsync();
            return View(dto);
        }

        var result = await _mediator.Send(new CreateSupplierPaymentCommand(dto, User.Identity?.Name));
        if (result != CreateSupplierPaymentResult.Success)
        {
            ModelState.AddModelError(string.Empty, result == CreateSupplierPaymentResult.ExceedsOutstanding
                ? "The payment amount exceeds the outstanding balance of the selected bill."
                : "The selected bill is not approved or is inactive, so it cannot be paid.");
            await PopulateAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SupplierPaymentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetSupplierPaymentActiveCommand(id, !isActive, User.Identity?.Name));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.AccountingPost)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostToAccounting(long id, CancellationToken cancellationToken)
    {
        if (!await CanPostAccountingAsync()) return Forbid();
        try
        {
            await _accounting.PostSourceAsync("SupplierPayment", id, User.Identity?.Name ?? "system", cancellationToken);
            TempData["StatusMessage"] = "Supplier payment posted to accounting.";
        }
        catch (Exception ex) { TempData["ErrorMessage"] = ex.InnerException?.Message ?? ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateAsync()
    {
        ViewBag.PayableBills = await _mediator.Send(new GetPayableBillsQuery());
    }
    private async Task<bool> CanPostAccountingAsync()
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idValue, out var userId) && await _db.AccountingPostingAssignments.AsNoTracking()
            .AnyAsync(a => a.UserId == userId && a.CanPostSupplierPayment);
    }
}
