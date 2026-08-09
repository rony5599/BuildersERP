using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Milestones;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.MilestoneView)]
public class MilestonesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateMilestoneDto> _createValidator;
    private readonly IValidator<UpdateMilestoneDto> _updateValidator;

    public MilestonesController(IMediator mediator, IValidator<CreateMilestoneDto> createValidator, IValidator<UpdateMilestoneDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(Guid? projectId)
    {
        var milestones = await _mediator.Send(new GetAllMilestonesQuery(projectId));
        return View(milestones);
    }

    [PermissionAuthorize(PermissionNames.MilestoneManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateMilestoneDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MilestoneManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMilestoneDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateMilestoneCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.MilestoneManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var milestone = await _mediator.Send(new GetMilestoneByIdQuery(id));
        if (milestone is null)
        {
            return NotFound();
        }

        var dto = new UpdateMilestoneDto
        {
            Id = milestone.Id,
            Name = milestone.Name,
            TargetDate = milestone.TargetDate,
            ActualDate = milestone.ActualDate,
            Status = milestone.Status,
            Remarks = milestone.Remarks,
            ProjectId = milestone.ProjectId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MilestoneManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateMilestoneDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateMilestoneCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MilestoneManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetMilestoneActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        ViewBag.Projects = new SelectList(await _mediator.Send(new GetAllProjectsQuery()), "Id", "Name");
    }
}
