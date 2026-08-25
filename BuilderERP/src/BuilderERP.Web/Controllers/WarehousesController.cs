using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Branches;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.Warehouses;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.WarehouseView)]
public class WarehousesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateWarehouseDto> _createValidator;
    private readonly IValidator<UpdateWarehouseDto> _updateValidator;

    public WarehousesController(IMediator mediator, IValidator<CreateWarehouseDto> createValidator, IValidator<UpdateWarehouseDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(long? projectId, int page = 1, int pageSize = 25)
    {
        var warehouses = await _mediator.Send(new GetAllWarehousesQuery(projectId, page, pageSize));
        ViewBag.SelectedProjectId = projectId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", warehouses);
        }

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);
        return View(warehouses);
    }

    [PermissionAuthorize(PermissionNames.WarehouseManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateWarehouseDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WarehouseManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWarehouseDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateWarehouseCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.WarehouseManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var warehouse = await _mediator.Send(new GetWarehouseByIdQuery(id));
        if (warehouse is null)
        {
            return NotFound();
        }

        var dto = new UpdateWarehouseDto
        {
            Id = warehouse.Id,
            WarehouseCode = warehouse.WarehouseCode,
            Name = warehouse.Name,
            Location = warehouse.Location,
            BranchId = warehouse.BranchId,
            ProjectId = warehouse.ProjectId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WarehouseManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateWarehouseDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateWarehouseCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WarehouseManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetWarehouseActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var branches = await _mediator.Send(new GetAllBranchesQuery(PageSize: int.MaxValue));
        ViewBag.Branches = new SelectList(branches.Items, "Id", "Name");

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
