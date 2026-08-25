using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.SecurityIncidents;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SecurityIncidentView)]
public class SecurityIncidentsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateSecurityIncidentDto> _createValidator;
    private readonly IValidator<UpdateSecurityIncidentDto> _updateValidator;

    public SecurityIncidentsController(IMediator mediator, IValidator<CreateSecurityIncidentDto> createValidator, IValidator<UpdateSecurityIncidentDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllSecurityIncidentsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.SecurityIncidentManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateSecurityIncidentDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SecurityIncidentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSecurityIncidentDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateSecurityIncidentCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.SecurityIncidentManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var item = await _mediator.Send(new GetSecurityIncidentByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateSecurityIncidentDto
        {
            Id = item.Id,
            IncidentNumber = item.IncidentNumber,
            IncidentType = item.IncidentType,
            Location = item.Location,
            IncidentDateTime = item.IncidentDateTime,
            ReportedBy = item.ReportedBy,
            Severity = item.Severity,
            Status = item.Status,
            ActionTaken = item.ActionTaken,
            Remarks = item.Remarks,
            ProjectId = item.ProjectId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SecurityIncidentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSecurityIncidentDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateSecurityIncidentCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SecurityIncidentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetSecurityIncidentActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
