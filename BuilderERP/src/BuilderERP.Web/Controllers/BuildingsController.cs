using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Buildings;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.BuildingView)]
public class BuildingsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateBuildingDto> _createValidator;
    private readonly IValidator<UpdateBuildingDto> _updateValidator;

    public BuildingsController(IMediator mediator, IValidator<CreateBuildingDto> createValidator, IValidator<UpdateBuildingDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var buildings = await _mediator.Send(new GetAllBuildingsQuery());
        return View(buildings);
    }

    [PermissionAuthorize(PermissionNames.BuildingManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateProjectsAsync();
        return View(new CreateBuildingDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BuildingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBuildingDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateProjectsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateBuildingCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.BuildingManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var building = await _mediator.Send(new GetBuildingByIdQuery(id));
        if (building is null)
        {
            return NotFound();
        }

        var dto = new UpdateBuildingDto
        {
            Id = building.Id,
            Name = building.Name,
            Code = building.Code,
            TotalFloors = building.TotalFloors,
            ProjectId = building.ProjectId
        };

        await PopulateProjectsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BuildingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateBuildingDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateProjectsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateBuildingCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BuildingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetBuildingActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateProjectsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery());
        ViewBag.Projects = new SelectList(projects, "Id", "Name");
    }
}
