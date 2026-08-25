using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.SiteInspections;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SiteInspectionView)]
public class SiteInspectionsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateSiteInspectionDto> _createValidator;
    private readonly IValidator<UpdateSiteInspectionDto> _updateValidator;

    public SiteInspectionsController(IMediator mediator, IValidator<CreateSiteInspectionDto> createValidator, IValidator<UpdateSiteInspectionDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var inspections = await _mediator.Send(new GetAllSiteInspectionsQuery(Page: page, PageSize: pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", inspections);
        }

        return View(inspections);
    }

    [PermissionAuthorize(PermissionNames.SiteInspectionManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateSiteInspectionDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SiteInspectionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSiteInspectionDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateSiteInspectionCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.SiteInspectionManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var inspection = await _mediator.Send(new GetSiteInspectionByIdQuery(id));
        if (inspection is null)
        {
            return NotFound();
        }

        var dto = new UpdateSiteInspectionDto
        {
            Id = inspection.Id,
            ProjectId = inspection.ProjectId,
            InspectionDate = inspection.InspectionDate,
            InspectedBy = inspection.InspectedBy,
            Location = inspection.Location,
            Result = inspection.Result,
            Remarks = inspection.Remarks
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SiteInspectionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSiteInspectionDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateSiteInspectionCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SiteInspectionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetSiteInspectionActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
