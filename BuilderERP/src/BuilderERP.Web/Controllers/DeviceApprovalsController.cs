using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Infrastructure.Persistence;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.DeviceApprovalView)]
public class DeviceApprovalsController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeviceApprovalsController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, DeviceStatus? status = DeviceStatus.Pending)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 25 : pageSize;

        var query = _context.UserDevices
            .Include(d => d.User)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(d => d.Status == status.Value);
        }

        query = query.OrderByDescending(d => d.RequestedDate);

        var totalCount = await query.CountAsync();
        var devices = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = devices.Select(d => new DeviceApprovalDto
        {
            Id = d.Id,
            UserId = d.UserId,
            UserName = d.User?.FullName ?? string.Empty,
            UserEmail = d.User?.Email ?? string.Empty,
            DeviceName = d.DeviceName,
            DeviceType = d.DeviceType,
            Browser = d.Browser,
            OperatingSystem = d.OperatingSystem,
            IPAddress = d.IPAddress,
            RequestedDate = d.RequestedDate,
            Status = d.Status
        }).ToList();

        var result = new PagedResult<DeviceApprovalDto>(dtos, totalCount, page, pageSize);
        ViewBag.StatusFilter = status;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", result);
        }

        return View(result);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DeviceApprovalManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(long id)
    {
        var device = await _context.UserDevices.FirstOrDefaultAsync(d => d.Id == id);
        if (device is null)
        {
            return NotFound();
        }

        device.Status = DeviceStatus.Approved;
        device.ApprovedDate = DateTime.UtcNow;
        device.ApprovedBy = ParseCurrentUserId();
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DeviceApprovalManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(long id)
    {
        var device = await _context.UserDevices.FirstOrDefaultAsync(d => d.Id == id);
        if (device is null)
        {
            return NotFound();
        }

        device.Status = DeviceStatus.Rejected;
        device.RejectedDate = DateTime.UtcNow;
        device.RejectedBy = ParseCurrentUserId();
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private Guid? ParseCurrentUserId()
    {
        var id = _userManager.GetUserId(User);
        return Guid.TryParse(id, out var guid) ? guid : null;
    }
}
