using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.DelayEvents;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.DelayEventView)]
public class DelayEventsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateDelayEventDto> _createValidator;
    private readonly IValidator<UpdateDelayEventDto> _updateValidator;

    public DelayEventsController(IMediator mediator, IValidator<CreateDelayEventDto> createValidator, IValidator<UpdateDelayEventDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(Guid? projectId, int page = 1, int pageSize = 25)
    {
        var delayEvents = await _mediator.Send(new GetAllDelayEventsQuery(projectId, page, pageSize));
        ViewBag.SelectedProjectId = projectId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", delayEvents);
        }

        return View(delayEvents);
    }

    [PermissionAuthorize(PermissionNames.DelayEventManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateDelayEventDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DelayEventManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDelayEventDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateDelayEventCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.DelayEventManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var delayEvent = await _mediator.Send(new GetDelayEventByIdQuery(id));
        if (delayEvent is null)
        {
            return NotFound();
        }

        var dto = new UpdateDelayEventDto
        {
            Id = delayEvent.Id,
            Description = delayEvent.Description,
            DelayDays = delayEvent.DelayDays,
            Reason = delayEvent.Reason,
            ReportedDate = delayEvent.ReportedDate,
            WbsTaskId = delayEvent.WbsTaskId,
            ProjectId = delayEvent.ProjectId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DelayEventManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateDelayEventDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateDelayEventCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DelayEventManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetDelayEventActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new BuilderERP.Application.Features.Projects.GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");

        var wbsTasks = await _mediator.Send(new BuilderERP.Application.Features.WbsTasks.GetAllWbsTasksQuery());
        ViewBag.WbsTasks = new SelectList(wbsTasks, "Id", "Code");
    }
}
