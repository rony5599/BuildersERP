using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.OperatorAssignments;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.Workers;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.OperatorAssignmentView)]
public class OperatorAssignmentsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateOperatorAssignmentDto> _createValidator;
    private readonly IValidator<UpdateOperatorAssignmentDto> _updateValidator;

    public OperatorAssignmentsController(IMediator mediator, IValidator<CreateOperatorAssignmentDto> createValidator, IValidator<UpdateOperatorAssignmentDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var assignments = await _mediator.Send(new GetAllOperatorAssignmentsQuery());
        return View(assignments);
    }

    [PermissionAuthorize(PermissionNames.OperatorAssignmentManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateOperatorAssignmentDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.OperatorAssignmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOperatorAssignmentDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateOperatorAssignmentCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.OperatorAssignmentManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var assignment = await _mediator.Send(new GetOperatorAssignmentByIdQuery(id));
        if (assignment is null)
        {
            return NotFound();
        }

        var dto = new UpdateOperatorAssignmentDto
        {
            Id = assignment.Id,
            AssignmentStartDate = assignment.AssignmentStartDate,
            AssignmentEndDate = assignment.AssignmentEndDate,
            EquipmentId = assignment.EquipmentId,
            WorkerId = assignment.WorkerId,
            ProjectId = assignment.ProjectId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.OperatorAssignmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateOperatorAssignmentDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateOperatorAssignmentCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.OperatorAssignmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetOperatorAssignmentActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var equipment = await _mediator.Send(new BuilderERP.Application.Features.Equipments.GetAllEquipmentQuery());
        ViewBag.Equipment = new SelectList(equipment, "Id", "Name");

        var workers = await _mediator.Send(new GetAllWorkersQuery());
        ViewBag.Workers = new SelectList(workers, "Id", "Name");

        var projects = await _mediator.Send(new GetAllProjectsQuery());
        ViewBag.Projects = new SelectList(projects, "Id", "Name");
    }
}
