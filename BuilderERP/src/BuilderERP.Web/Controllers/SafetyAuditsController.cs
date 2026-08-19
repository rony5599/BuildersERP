using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.SafetyAudits;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SafetyAuditView)]
public class SafetyAuditsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateSafetyAuditDto> _createValidator;
    private readonly IValidator<UpdateSafetyAuditDto> _updateValidator;

    public SafetyAuditsController(IMediator mediator, IValidator<CreateSafetyAuditDto> createValidator, IValidator<UpdateSafetyAuditDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllSafetyAuditsQuery(Page: page, PageSize: pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.SafetyAuditManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateSafetyAuditDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SafetyAuditManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSafetyAuditDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateSafetyAuditCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.SafetyAuditManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetSafetyAuditByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateSafetyAuditDto
        {
            Id = item.Id,
            ProjectId = item.ProjectId,
            AuditDate = item.AuditDate,
            AuditedBy = item.AuditedBy,
            Score = item.Score,
            Findings = item.Findings,
            Status = item.Status
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SafetyAuditManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSafetyAuditDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateSafetyAuditCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SafetyAuditManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetSafetyAuditActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
