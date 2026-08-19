using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.MaterialInspections;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.MaterialInspectionView)]
public class MaterialInspectionsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateMaterialInspectionDto> _createValidator;
    private readonly IValidator<UpdateMaterialInspectionDto> _updateValidator;

    public MaterialInspectionsController(IMediator mediator, IValidator<CreateMaterialInspectionDto> createValidator, IValidator<UpdateMaterialInspectionDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var inspections = await _mediator.Send(new GetAllMaterialInspectionsQuery(Page: page, PageSize: pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", inspections);
        }

        return View(inspections);
    }

    [PermissionAuthorize(PermissionNames.MaterialInspectionManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateMaterialInspectionDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaterialInspectionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMaterialInspectionDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateMaterialInspectionCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.MaterialInspectionManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var inspection = await _mediator.Send(new GetMaterialInspectionByIdQuery(id));
        if (inspection is null)
        {
            return NotFound();
        }

        var dto = new UpdateMaterialInspectionDto
        {
            Id = inspection.Id,
            ProjectId = inspection.ProjectId,
            MaterialId = inspection.MaterialId,
            InspectionDate = inspection.InspectionDate,
            InspectedBy = inspection.InspectedBy,
            Quantity = inspection.Quantity,
            Result = inspection.Result,
            Remarks = inspection.Remarks
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaterialInspectionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateMaterialInspectionDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateMaterialInspectionCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaterialInspectionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetMaterialInspectionActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");

        var materials = await _mediator.Send(new GetAllMaterialsQuery(PageSize: int.MaxValue));
        ViewBag.Materials = new SelectList(materials.Items, "Id", "Name");
    }
}
