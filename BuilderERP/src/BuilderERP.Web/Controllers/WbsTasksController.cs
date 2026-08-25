using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Application.Features.WbsTasks;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.WbsTaskView)]
public class WbsTasksController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateWbsTaskDto> _createValidator;
    private readonly IValidator<UpdateWbsTaskDto> _updateValidator;

    public WbsTasksController(IMediator mediator, IValidator<CreateWbsTaskDto> createValidator, IValidator<UpdateWbsTaskDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(long? projectId, long? propertyUnitId)
    {
        var wbsTasks = await _mediator.Send(new GetAllWbsTasksQuery(projectId, propertyUnitId));
        ViewBag.Projects = new SelectList((await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue))).Items, "Id", "Name", projectId);
        ViewBag.PropertyUnits = new SelectList((await _mediator.Send(new GetAllPropertyUnitsQuery(projectId, PageSize: int.MaxValue))).Items, "Id", "UnitNumber", propertyUnitId);
        ViewBag.SelectedProjectId = projectId;
        ViewBag.SelectedPropertyUnitId = propertyUnitId;
        return View(wbsTasks);
    }

    public async Task<IActionResult> Gantt(long? projectId)
    {
        ViewBag.Projects = new SelectList((await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue))).Items, "Id", "Name", projectId);
        var wbsTasks = await _mediator.Send(new GetAllWbsTasksQuery(projectId));
        return View(wbsTasks);
    }

    public async Task<IActionResult> GanttByUnit(long? projectId, long? propertyUnitId)
    {
        ViewBag.Projects = new SelectList((await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue))).Items, "Id", "Name", projectId);
        ViewBag.PropertyUnits = new SelectList((await _mediator.Send(new GetAllPropertyUnitsQuery(projectId, PageSize: int.MaxValue))).Items, "Id", "UnitNumber", propertyUnitId);
        ViewBag.SelectedProjectId = projectId;
        ViewBag.SelectedPropertyUnitId = propertyUnitId;

        var wbsTasks = propertyUnitId.HasValue
            ? await _mediator.Send(new GetAllWbsTasksQuery(null, propertyUnitId))
            : new List<WbsTaskDto>();

        return View(wbsTasks);
    }

    [PermissionAuthorize(PermissionNames.WbsTaskManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateWbsTaskDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WbsTaskManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWbsTaskDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateWbsTaskCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.WbsTaskManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var wbsTask = await _mediator.Send(new GetWbsTaskByIdQuery(id));
        if (wbsTask is null)
        {
            return NotFound();
        }

        var dto = new UpdateWbsTaskDto
        {
            Id = wbsTask.Id,
            Code = wbsTask.Code,
            Name = wbsTask.Name,
            Description = wbsTask.Description,
            StartDate = wbsTask.StartDate,
            EndDate = wbsTask.EndDate,
            PercentComplete = wbsTask.PercentComplete,
            Sequence = wbsTask.Sequence,
            ParentId = wbsTask.ParentId,
            ProjectId = wbsTask.ProjectId,
            PropertyUnitId = wbsTask.PropertyUnitId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WbsTaskManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateWbsTaskDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateWbsTaskCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WbsTaskManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetWbsTaskActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        ViewBag.Projects = new SelectList((await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue))).Items, "Id", "Name");
        ViewBag.ParentTasks = new SelectList(await _mediator.Send(new GetAllWbsTasksQuery()), "Id", "Code");
        ViewBag.PropertyUnits = new SelectList((await _mediator.Send(new GetAllPropertyUnitsQuery(PageSize: int.MaxValue))).Items, "Id", "UnitNumber");
    }
}
