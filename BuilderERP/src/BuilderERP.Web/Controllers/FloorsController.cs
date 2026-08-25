using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Floors;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.Towers;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.FloorView)]
public class FloorsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateFloorDto> _createValidator;
    private readonly IValidator<UpdateFloorDto> _updateValidator;

    public FloorsController(IMediator mediator, IValidator<CreateFloorDto> createValidator, IValidator<UpdateFloorDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(long? projectId, int page = 1, int pageSize = 25)
    {
        var floors = await _mediator.Send(new GetAllFloorsQuery(projectId, page, pageSize));
        ViewBag.SelectedProjectId = projectId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", floors);
        }

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);
        return View(floors);
    }

    [PermissionAuthorize(PermissionNames.FloorManage)]
    public async Task<IActionResult> Create(long? projectId)
    {
        await PopulateTowersAsync(projectId);
        return View(new CreateFloorDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.FloorManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateFloorDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateTowersAsync(null);
            return View(dto);
        }

        await _mediator.Send(new CreateFloorCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.FloorManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var floor = await _mediator.Send(new GetFloorByIdQuery(id));
        if (floor is null)
        {
            return NotFound();
        }

        var dto = new UpdateFloorDto
        {
            Id = floor.Id,
            Name = floor.Name,
            FloorNumber = floor.FloorNumber,
            TowerId = floor.TowerId
        };

        await PopulateTowersAsync(null);
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.FloorManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateFloorDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateTowersAsync(null);
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateFloorCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.FloorManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetFloorActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateTowersAsync(long? projectId)
    {
        var towers = await _mediator.Send(new GetAllTowersQuery(projectId, PageSize: int.MaxValue));
        ViewBag.Towers = new SelectList(towers.Items, "Id", "Name");
    }
}
