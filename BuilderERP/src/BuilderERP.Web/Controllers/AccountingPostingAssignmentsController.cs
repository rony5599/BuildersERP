using BuilderERP.Domain.Entities;
using BuilderERP.Infrastructure.Persistence;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.UserManage)]
public sealed class AccountingPostingAssignmentsController(AppDbContext db, UserManager<ApplicationUser> userManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var assignments = await db.AccountingPostingAssignments.AsNoTracking().ToDictionaryAsync(a => a.UserId);
        var users = await userManager.Users.Where(u => u.IsActive).OrderBy(u => u.FullName).ToListAsync();
        var model = new AccountingPostingAssignmentPageViewModel();
        foreach (var user in users)
        {
            assignments.TryGetValue(user.Id, out var a);
            var roles = await userManager.GetRolesAsync(user);
            model.Users.Add(new AccountingPostingAssignmentRowViewModel
            {
                UserId = user.Id, FullName = user.FullName, Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? "User", CanPostPoBill = a?.CanPostPoBill ?? false,
                CanPostEwoBill = a?.CanPostEwoBill ?? false, CanPostSupplierPayment = a?.CanPostSupplierPayment ?? false,
                CanPostCustomerReceipt = a?.CanPostCustomerReceipt ?? false,
                CanPostManualJournal = a?.CanPostManualJournal ?? false, CanReverse = a?.CanReverse ?? false
            });
        }
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AccountingPostingAssignmentPageViewModel model)
    {
        var validIds = (await userManager.Users.Where(u => u.IsActive).Select(u => u.Id).ToListAsync()).ToHashSet();
        var existing = await db.AccountingPostingAssignments.ToDictionaryAsync(a => a.UserId);
        foreach (var row in model.Users.Where(r => validIds.Contains(r.UserId)))
        {
            if (!existing.TryGetValue(row.UserId, out var a))
            {
                a = new AccountingPostingAssignment { UserId = row.UserId, CreatedBy = User.Identity?.Name };
                db.AccountingPostingAssignments.Add(a);
            }
            a.CanPostPoBill = row.CanPostPoBill; a.CanPostEwoBill = row.CanPostEwoBill;
            a.CanPostSupplierPayment = row.CanPostSupplierPayment; a.CanPostCustomerReceipt = row.CanPostCustomerReceipt;
            a.CanPostManualJournal = row.CanPostManualJournal; a.CanReverse = row.CanReverse;
            a.ModifiedAt = DateTime.UtcNow; a.ModifiedBy = User.Identity?.Name;
        }
        await db.SaveChangesAsync();
        TempData["StatusMessage"] = "Accounting posting assignments saved.";
        return RedirectToAction(nameof(Index));
    }
}
