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
public class CashPoBillActionAssignmentsController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public CashPoBillActionAssignmentsController(AppDbContext db, UserManager<ApplicationUser> userManager) { _db = db; _userManager = userManager; }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var assignments = await _db.CashPoBillActionAssignments.AsNoTracking().ToDictionaryAsync(a => a.UserId);
        var users = await _userManager.Users.Where(u => u.IsActive).OrderBy(u => u.FullName).ToListAsync();
        var model = new CashPoBillActionAssignmentPageViewModel();
        foreach (var user in users)
        {
            assignments.TryGetValue(user.Id, out var assignment);
            var roles = await _userManager.GetRolesAsync(user);
            model.Users.Add(new CashPoBillActionAssignmentRowViewModel
            {
                UserId = user.Id, FullName = user.FullName, Email = user.Email ?? string.Empty, Role = roles.FirstOrDefault() ?? "User",
                CanDraftEdit = assignment?.CanDraftEdit ?? false, CanSubmit = assignment?.CanSubmit ?? false,
                CanRequestApproval = assignment?.CanRequestApproval ?? false, CanApprove = assignment?.CanApprove ?? false,
                CanReject = assignment?.CanReject ?? false, CanCancel = assignment?.CanCancel ?? false
            });
        }
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CashPoBillActionAssignmentPageViewModel model)
    {
        var validIds = (await _userManager.Users.Where(u => u.IsActive).Select(u => u.Id).ToListAsync()).ToHashSet();
        var existing = await _db.CashPoBillActionAssignments.ToDictionaryAsync(a => a.UserId);
        foreach (var row in model.Users.Where(r => validIds.Contains(r.UserId)))
        {
            if (!existing.TryGetValue(row.UserId, out var a)) { a = new CashPoBillActionAssignment { UserId = row.UserId }; _db.Add(a); }
            a.CanDraftEdit = row.CanDraftEdit; a.CanSubmit = row.CanSubmit; a.CanRequestApproval = row.CanRequestApproval;
            a.CanApprove = row.CanApprove; a.CanReject = row.CanReject; a.CanCancel = row.CanCancel;
        }
        await _db.SaveChangesAsync();
        TempData["StatusMessage"] = "Cash PO Bill action assignments saved.";
        return RedirectToAction(nameof(Index));
    }
}
