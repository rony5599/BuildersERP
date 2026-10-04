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
public class EngineerWorkOrderActionAssignmentsController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public EngineerWorkOrderActionAssignmentsController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var assignments = await _db.EngineerWorkOrderActionAssignments.AsNoTracking()
            .ToDictionaryAsync(a => a.UserId);
        var users = await _userManager.Users.Where(u => u.IsActive).OrderBy(u => u.FullName).ToListAsync();
        var model = new EngineerWorkOrderActionAssignmentPageViewModel();

        foreach (var user in users)
        {
            assignments.TryGetValue(user.Id, out var assignment);
            var roles = await _userManager.GetRolesAsync(user);
            model.Users.Add(new EngineerWorkOrderActionAssignmentRowViewModel
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? "User",
                CanDraftEdit = assignment?.CanDraftEdit ?? false,
                CanSubmit = assignment?.CanSubmit ?? false,
                CanRequestApproval = assignment?.CanRequestApproval ?? false,
                CanApprove = assignment?.CanApprove ?? false,
                CanReject = assignment?.CanReject ?? false,
                CanCancel = assignment?.CanCancel ?? false
            });
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(EngineerWorkOrderActionAssignmentPageViewModel model)
    {
        var validUserIds = (await _userManager.Users.Where(u => u.IsActive).Select(u => u.Id).ToListAsync()).ToHashSet();
        var existing = await _db.EngineerWorkOrderActionAssignments.ToDictionaryAsync(a => a.UserId);

        foreach (var row in model.Users.Where(r => validUserIds.Contains(r.UserId)))
        {
            if (!existing.TryGetValue(row.UserId, out var assignment))
            {
                assignment = new EngineerWorkOrderActionAssignment { UserId = row.UserId };
                _db.EngineerWorkOrderActionAssignments.Add(assignment);
            }

            assignment.CanDraftEdit = row.CanDraftEdit;
            assignment.CanSubmit = row.CanSubmit;
            assignment.CanRequestApproval = row.CanRequestApproval;
            assignment.CanApprove = row.CanApprove;
            assignment.CanReject = row.CanReject;
            assignment.CanCancel = row.CanCancel;
        }

        await _db.SaveChangesAsync();
        TempData["StatusMessage"] = "Engineer Work Order action assignments saved.";
        return RedirectToAction(nameof(Index));
    }
}
