using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Overtimes;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.OvertimeView)]
public class OvertimesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateOvertimeDto> _createValidator;
    private readonly IValidator<UpdateOvertimeDto> _updateValidator;

    public OvertimesController(IMediator mediator, IValidator<CreateOvertimeDto> createValidator, IValidator<UpdateOvertimeDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var overtimes = await _mediator.Send(new GetAllOvertimesQuery());
        return View(overtimes);
    }

    [PermissionAuthorize(PermissionNames.OvertimeManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateOvertimeDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.OvertimeManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOvertimeDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateOvertimeCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.OvertimeManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var overtime = await _mediator.Send(new GetOvertimeByIdQuery(id));
        if (overtime is null)
        {
            return NotFound();
        }

        var dto = new UpdateOvertimeDto
        {
            Id = overtime.Id,
            OvertimeDate = overtime.OvertimeDate,
            Hours = overtime.Hours,
            RatePerHour = overtime.RatePerHour,
            WorkerId = overtime.WorkerId,
            ProjectId = overtime.ProjectId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.OvertimeManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateOvertimeDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateOvertimeCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.OvertimeManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetOvertimeActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var workers = await _mediator.Send(new BuilderERP.Application.Features.Workers.GetAllWorkersQuery());
        ViewBag.Workers = new SelectList(workers, "Id", "Name");

        var projects = await _mediator.Send(new BuilderERP.Application.Features.Projects.GetAllProjectsQuery());
        ViewBag.Projects = new SelectList(projects, "Id", "Name");
    }
}
