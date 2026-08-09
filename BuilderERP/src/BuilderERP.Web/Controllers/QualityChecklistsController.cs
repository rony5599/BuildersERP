using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.QualityChecklists;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.QualityChecklistView)]
public class QualityChecklistsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateQualityChecklistDto> _createValidator;
    private readonly IValidator<UpdateQualityChecklistDto> _updateValidator;

    public QualityChecklistsController(IMediator mediator, IValidator<CreateQualityChecklistDto> createValidator, IValidator<UpdateQualityChecklistDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var checklists = await _mediator.Send(new GetAllQualityChecklistsQuery());
        return View(checklists);
    }

    [PermissionAuthorize(PermissionNames.QualityChecklistManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateQualityChecklistDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.QualityChecklistManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateQualityChecklistDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateQualityChecklistCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.QualityChecklistManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var checklist = await _mediator.Send(new GetQualityChecklistByIdQuery(id));
        if (checklist is null)
        {
            return NotFound();
        }

        var dto = new UpdateQualityChecklistDto
        {
            Id = checklist.Id,
            ProjectId = checklist.ProjectId,
            ChecklistName = checklist.ChecklistName,
            Category = checklist.Category,
            ChecklistDate = checklist.ChecklistDate,
            CheckedBy = checklist.CheckedBy,
            Result = checklist.Result,
            Remarks = checklist.Remarks
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.QualityChecklistManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateQualityChecklistDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateQualityChecklistCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.QualityChecklistManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetQualityChecklistActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery());
        ViewBag.Projects = new SelectList(projects, "Id", "Name");
    }
}
