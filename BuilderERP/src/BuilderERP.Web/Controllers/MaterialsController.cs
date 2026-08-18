using BuilderERP.Application.DTOs;
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

[PermissionAuthorize(PermissionNames.MaterialView)]
public class MaterialsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateMaterialDto> _createValidator;
    private readonly IValidator<UpdateMaterialDto> _updateValidator;

    public MaterialsController(IMediator mediator, IValidator<CreateMaterialDto> createValidator, IValidator<UpdateMaterialDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(Guid? projectId)
    {
        var materials = await _mediator.Send(new GetAllMaterialsQuery(projectId));
        var projects = await _mediator.Send(new GetAllProjectsQuery());
        ViewBag.Projects = new SelectList(projects, "Id", "Name", projectId);
        ViewBag.SelectedProjectId = projectId;
        return View(materials);
    }

    [PermissionAuthorize(PermissionNames.MaterialManage)]
    public IActionResult Create()
    {
        return View(new CreateMaterialDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaterialManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMaterialDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }

        await _mediator.Send(new CreateMaterialCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.MaterialManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var material = await _mediator.Send(new GetMaterialByIdQuery(id));
        if (material is null)
        {
            return NotFound();
        }

        var dto = new UpdateMaterialDto
        {
            Id = material.Id,
            MaterialCode = material.MaterialCode,
            Name = material.Name,
            Description = material.Description,
            UnitOfMeasure = material.UnitOfMeasure,
            ReorderLevel = material.ReorderLevel,
            Barcode = material.Barcode
        };

        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaterialManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateMaterialDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateMaterialCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaterialManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetMaterialActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }
}
