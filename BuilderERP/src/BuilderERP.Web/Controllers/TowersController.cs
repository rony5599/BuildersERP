using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Buildings;
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

[PermissionAuthorize(PermissionNames.TowerView)]
public class TowersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateTowerDto> _createValidator;
    private readonly IValidator<UpdateTowerDto> _updateValidator;

    public TowersController(IMediator mediator, IValidator<CreateTowerDto> createValidator, IValidator<UpdateTowerDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(Guid? projectId)
    {
        var towers = await _mediator.Send(new GetAllTowersQuery(projectId));
        var projects = await _mediator.Send(new GetAllProjectsQuery());
        ViewBag.Projects = new SelectList(projects, "Id", "Name", projectId);
        ViewBag.SelectedProjectId = projectId;
        return View(towers);
    }

    [PermissionAuthorize(PermissionNames.TowerManage)]
    public async Task<IActionResult> Create(Guid? projectId)
    {
        await PopulateBuildingsAsync(projectId);
        return View(new CreateTowerDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.TowerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTowerDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateBuildingsAsync(null);
            return View(dto);
        }

        await _mediator.Send(new CreateTowerCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.TowerManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var tower = await _mediator.Send(new GetTowerByIdQuery(id));
        if (tower is null)
        {
            return NotFound();
        }

        var dto = new UpdateTowerDto
        {
            Id = tower.Id,
            Name = tower.Name,
            Code = tower.Code,
            BuildingId = tower.BuildingId
        };

        await PopulateBuildingsAsync(null);
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.TowerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateTowerDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateBuildingsAsync(null);
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateTowerCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.TowerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetTowerActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateBuildingsAsync(Guid? projectId)
    {
        var buildings = await _mediator.Send(new GetAllBuildingsQuery(projectId));
        ViewBag.Buildings = new SelectList(buildings, "Id", "Name");
    }
}
