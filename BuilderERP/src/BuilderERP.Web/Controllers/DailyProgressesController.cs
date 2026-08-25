using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.DailyProgresses;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.DailyProgressView)]
public class DailyProgressesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateDailyProgressDto> _createValidator;
    private readonly IValidator<UpdateDailyProgressDto> _updateValidator;

    public DailyProgressesController(IMediator mediator, IValidator<CreateDailyProgressDto> createValidator, IValidator<UpdateDailyProgressDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(long? projectId, int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllDailyProgressesQuery(projectId, page, pageSize));
        ViewBag.SelectedProjectId = projectId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.DailyProgressManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateDailyProgressDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DailyProgressManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDailyProgressDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateDailyProgressCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.DailyProgressManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var item = await _mediator.Send(new GetDailyProgressByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateDailyProgressDto
        {
            Id = item.Id,
            ProgressDate = item.ProgressDate,
            Description = item.Description,
            PercentComplete = item.PercentComplete,
            ManpowerCount = item.ManpowerCount,
            Remarks = item.Remarks,
            ProjectId = item.ProjectId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DailyProgressManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateDailyProgressDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateDailyProgressCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DailyProgressManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetDailyProgressActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
