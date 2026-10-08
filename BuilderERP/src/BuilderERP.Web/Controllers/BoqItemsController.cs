using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.BoqItems;
using BuilderERP.Application.Features.Projects;
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

[PermissionAuthorize(PermissionNames.BoqItemView)]
public class BoqItemsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<UpdateBoqDto> _updateValidator;
    private readonly IValidator<CreateBoqDto> _createBoqValidator;
    private readonly AppDbContext _db;

    public BoqItemsController(IMediator mediator, IValidator<UpdateBoqDto> updateValidator, IValidator<CreateBoqDto> createBoqValidator, AppDbContext db)
    {
        _mediator = mediator;
        _updateValidator = updateValidator;
        _createBoqValidator = createBoqValidator;
        _db = db;
    }

    public async Task<IActionResult> Index(long? projectId, string? boqName, int page = 1, int pageSize = 25)
    {
        ViewBag.ActionAssignment = await GetCurrentAssignmentAsync();
        pageSize = pageSize is 10 or 25 or 50 or 100 ? pageSize : 25;
        var items = await _mediator.Send(new GetAllBoqsQuery(projectId, boqName, page, pageSize));
        ViewBag.SelectedProjectId = projectId;
        ViewBag.SelectedBoqName = boqName?.Trim();
        ViewBag.SelectedPageSize = pageSize;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);
        return View(items);
    }

    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    public async Task<IActionResult> Create()
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        await PopulateDropdownsAsync();
        return View(new CreateBoqDto { VersionNumber = 1, Items = new List<CreateBoqItemDto> { new() } });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBoqDto dto)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        var validationResult = await _createBoqValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateBoqCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    public async Task<IActionResult> Edit(long id)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        var dto = await _mediator.Send(new GetBoqForEditQuery(id));
        if (dto is null) return NotFound();
        if (dto.Status != BoqStatus.Draft)
        {
            TempData["ErrorMessage"] = "Only Draft BOQs can be edited.";
            return RedirectToAction(nameof(Index));
        }
        await PopulateDropdownsAsync();
        ViewBag.IsEdit = true;
        return View("Create", dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateBoqDto dto)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            ViewBag.IsEdit = true;
            return View("Create", dto);
        }

        var success = await _mediator.Send(new UpdateBoqCommand(dto));
        if (!success)
        {
            TempData["ErrorMessage"] = "The BOQ was not found or is no longer editable.";
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        if (!await HasActionAsync(a => a.CanDraftEdit)) return Forbid();
        await _mediator.Send(new SetBoqItemActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WorkflowAction(long id, string workflowAction, string? rejectionReason)
    {
        var assignment = await GetCurrentAssignmentAsync();
        if (assignment is null) return Forbid();
        var boq = await _db.BoqHeaders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (boq is null) return NotFound();

        var action = workflowAction?.Trim().ToLowerInvariant();
        var allowed = action switch { "submit" => assignment.CanSubmit, "request" => assignment.CanRequestApproval,
            "approve" => assignment.CanApprove, "reject" => assignment.CanReject, "cancel" => assignment.CanCancel, _ => false };
        if (!allowed) { TempData["ErrorMessage"] = "You are not assigned to perform that action."; return RedirectToAction(nameof(Index)); }
        if (action == "reject" && string.IsNullOrWhiteSpace(rejectionReason))
        { TempData["ErrorMessage"] = "A rejection reason is required."; return RedirectToAction(nameof(Index)); }
        if (rejectionReason?.Length > 1000)
        { TempData["ErrorMessage"] = "The rejection reason cannot exceed 1,000 characters."; return RedirectToAction(nameof(Index)); }

        var valid = true;
        var to = boq.Status;
        switch (action)
        {
            case "submit" when boq.Status == BoqStatus.Draft: to = BoqStatus.Submitted; break;
            case "request" when boq.Status == BoqStatus.Submitted: to = BoqStatus.AwaitingApproval; break;
            case "approve" when boq.Status == BoqStatus.AwaitingApproval: to = BoqStatus.Approved; break;
            case "reject" when boq.Status is BoqStatus.Submitted or BoqStatus.AwaitingApproval or BoqStatus.Approved: to = BoqStatus.Draft; break;
            case "cancel" when boq.Status is BoqStatus.Draft or BoqStatus.Submitted or BoqStatus.AwaitingApproval: to = BoqStatus.Cancelled; break;
            default: valid = false; break;
        }
        if (!valid) { TempData["ErrorMessage"] = $"That action is not available while this BOQ is {boq.Status}."; return RedirectToAction(nameof(Index)); }

        var updated = await _mediator.Send(new SetBoqStatusCommand(id, boq.Status, to, User.Identity?.Name,
            action == "reject" ? rejectionReason : null));
        if (!updated) { TempData["ErrorMessage"] = "The BOQ changed while the action was being processed. Please try again."; return RedirectToAction(nameof(Index)); }
        TempData["StatusMessage"] = action == "reject" ? "BOQ returned to Draft for revision." : $"BOQ is now {to}.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
        ViewBag.WorkGroups = await _mediator.Send(new GetWorkGroupOptionsQuery());
    }

    private async Task<BoqActionAssignment?> GetCurrentAssignmentAsync()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId)
            ? await _db.BoqActionAssignments.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == userId)
            : null;
    }

    private async Task<bool> HasActionAsync(Func<BoqActionAssignment, bool> predicate)
    {
        var assignment = await GetCurrentAssignmentAsync();
        return assignment is not null && predicate(assignment);
    }

}
