using BuilderERP.Application.Features.Accounting;
using BuilderERP.Domain.Entities;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BuilderERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.AccountingView)]
public sealed class AccountingController(IAccountingService accounting, AppDbContext db) : Controller
{
    public async Task<IActionResult> Accounts(CancellationToken ct) => View(await accounting.GetAccountsAsync(false, ct));

    [PermissionAuthorize(PermissionNames.AccountingManage)]
    public async Task<IActionResult> Account(long? id, CancellationToken ct)
    {
        var accounts = await accounting.GetAccountsAsync(false, ct);
        var existing = id.HasValue ? accounts.FirstOrDefault(a => a.Id == id) : null;
        ViewBag.Parents = new SelectList(accounts.Where(a => a.Id != id), "Id", "Name", existing?.ParentId);
        return View(new LedgerAccountInput
        {
            Id = existing?.Id, Code = existing?.Code ?? "", Name = existing?.Name ?? "",
            AccountType = existing?.AccountType ?? AccountType.Asset, ParentId = existing?.ParentId,
            AllowPosting = existing?.AllowPosting ?? true, IsActive = existing?.IsActive ?? true
        });
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize(PermissionNames.AccountingManage)]
    public async Task<IActionResult> Account(LedgerAccountInput input, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(input.Code) || string.IsNullOrWhiteSpace(input.Name))
            ModelState.AddModelError("", "Code and name are required.");
        if (ModelState.IsValid)
        {
            try { await accounting.SaveAccountAsync(input, User.Identity?.Name ?? "system", ct); return RedirectToAction(nameof(Accounts)); }
            catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
        }
        ViewBag.Parents = new SelectList(await accounting.GetAccountsAsync(false, ct), "Id", "Name", input.ParentId);
        return View(input);
    }

    public async Task<IActionResult> Journals(CancellationToken ct) => View(await accounting.GetVouchersAsync(ct));

    [PermissionAuthorize(PermissionNames.AccountingManage)]
    public async Task<IActionResult> CreateJournal(CancellationToken ct)
    {
        await PopulateAccounts(ct);
        return View(new JournalVoucherInput());
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize(PermissionNames.AccountingManage)]
    public async Task<IActionResult> CreateJournal(JournalVoucherInput input, CancellationToken ct)
    {
        try
        {
            var id = await accounting.CreateDraftAsync(input, User.Identity?.Name ?? "system", ct);
            return RedirectToAction(nameof(Journal), new { id });
        }
        catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
        await PopulateAccounts(ct);
        return View(input);
    }

    public async Task<IActionResult> Journal(long id, CancellationToken ct)
    {
        var voucher = await accounting.GetVoucherAsync(id, ct);
        var assignment = await GetPostingAssignmentAsync(ct);
        ViewBag.CanPostManualJournal = assignment?.CanPostManualJournal == true;
        ViewBag.CanReverse = assignment?.CanReverse == true;
        return voucher is null ? NotFound() : View(voucher);
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize(PermissionNames.AccountingPost)]
    public async Task<IActionResult> Post(long id, CancellationToken ct)
    {
        if ((await GetPostingAssignmentAsync(ct))?.CanPostManualJournal != true) return Forbid();
        try { await accounting.PostAsync(id, User.Identity?.Name ?? "system", ct); TempData["Success"] = "Voucher posted."; }
        catch (Exception ex) { TempData["Error"] = ex.InnerException?.Message ?? ex.Message; }
        return RedirectToAction(nameof(Journal), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize(PermissionNames.AccountingPost)]
    public async Task<IActionResult> Reverse(long id, DateTime reversalDate, string reason, CancellationToken ct)
    {
        if ((await GetPostingAssignmentAsync(ct))?.CanReverse != true) return Forbid();
        try
        {
            var reversalId = await accounting.ReverseAsync(id, reversalDate, reason, User.Identity?.Name ?? "system", ct);
            TempData["Success"] = "Reversal voucher created and posted.";
            return RedirectToAction(nameof(Journal), new { id = reversalId });
        }
        catch (Exception ex) { TempData["Error"] = ex.InnerException?.Message ?? ex.Message; return RedirectToAction(nameof(Journal), new { id }); }
    }

    public async Task<IActionResult> TrialBalance(DateTime? fromDate, DateTime? toDate, long? projectId, long? costCenterId, CancellationToken ct)
    {
        var from = fromDate ?? new DateTime(DateTime.Today.Year, 1, 1);
        var to = toDate ?? DateTime.Today;
        ViewBag.FromDate = from; ViewBag.ToDate = to;
        return View(await accounting.GetTrialBalanceAsync(from, to, projectId, costCenterId, ct));
    }

    private async Task PopulateAccounts(CancellationToken ct) =>
        ViewBag.Accounts = new SelectList((await accounting.GetAccountsAsync(true, ct)).Select(a => new { a.Id, Name = $"{a.Code} - {a.Name}" }), "Id", "Name");

    private async Task<AccountingPostingAssignment?> GetPostingAssignmentAsync(CancellationToken ct)
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idValue, out var userId)
            ? await db.AccountingPostingAssignments.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == userId, ct)
            : null;
    }
}
