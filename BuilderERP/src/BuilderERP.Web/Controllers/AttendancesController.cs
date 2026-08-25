using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Attendances;
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

[PermissionAuthorize(PermissionNames.AttendanceView)]
public class AttendancesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateAttendanceDto> _createValidator;
    private readonly IValidator<UpdateAttendanceDto> _updateValidator;

    public AttendancesController(IMediator mediator, IValidator<CreateAttendanceDto> createValidator, IValidator<UpdateAttendanceDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var attendances = await _mediator.Send(new GetAllAttendancesQuery(Page: page, PageSize: pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", attendances);
        }

        return View(attendances);
    }

    [PermissionAuthorize(PermissionNames.AttendanceManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateAttendanceDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.AttendanceManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAttendanceDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateAttendanceCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.AttendanceManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var attendance = await _mediator.Send(new GetAttendanceByIdQuery(id));
        if (attendance is null)
        {
            return NotFound();
        }

        var dto = new UpdateAttendanceDto
        {
            Id = attendance.Id,
            WorkerId = attendance.WorkerId,
            ProjectId = attendance.ProjectId,
            AttendanceDate = attendance.AttendanceDate,
            Status = attendance.Status,
            HoursWorked = attendance.HoursWorked
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.AttendanceManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateAttendanceDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateAttendanceCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.AttendanceManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetAttendanceActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var workers = await _mediator.Send(new GetAllWorkersQuery(PageSize: int.MaxValue));
        ViewBag.Workers = new SelectList(workers.Items, "Id", "Name");

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
