using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.SafetyInspections;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SafetyInspectionView)]
public class SafetyInspectionsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateSafetyInspectionDto> _createValidator;
    private readonly IValidator<UpdateSafetyInspectionDto> _updateValidator;

    public SafetyInspectionsController(IMediator mediator, IValidator<CreateSafetyInspectionDto> createValidator, IValidator<UpdateSafetyInspectionDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllSafetyInspectionsQuery(Page: page, PageSize: pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.SafetyInspectionManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateSafetyInspectionDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SafetyInspectionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSafetyInspectionDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateSafetyInspectionCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.SafetyInspectionManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var item = await _mediator.Send(new GetSafetyInspectionByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateSafetyInspectionDto
        {
            Id = item.Id,
            ProjectId = item.ProjectId,
            InspectionDate = item.InspectionDate,
            InspectedBy = item.InspectedBy,
            Location = item.Location,
            Result = item.Result,
            Remarks = item.Remarks
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SafetyInspectionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSafetyInspectionDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateSafetyInspectionCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SafetyInspectionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetSafetyInspectionActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
