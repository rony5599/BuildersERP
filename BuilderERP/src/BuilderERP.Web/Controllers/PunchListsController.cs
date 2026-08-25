using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.PunchLists;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.PunchListView)]
public class PunchListsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreatePunchListDto> _createValidator;
    private readonly IValidator<UpdatePunchListDto> _updateValidator;

    public PunchListsController(IMediator mediator, IValidator<CreatePunchListDto> createValidator, IValidator<UpdatePunchListDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(long? projectId, int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllPunchListsQuery(projectId, page, pageSize));
        ViewBag.SelectedProjectId = projectId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.PunchListManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreatePunchListDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PunchListManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePunchListDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreatePunchListCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.PunchListManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var item = await _mediator.Send(new GetPunchListByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdatePunchListDto
        {
            Id = item.Id,
            ItemNumber = item.ItemNumber,
            Location = item.Location,
            Description = item.Description,
            Status = item.Status,
            AssignedTo = item.AssignedTo,
            DueDate = item.DueDate,
            CompletedDate = item.CompletedDate,
            ProjectId = item.ProjectId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PunchListManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdatePunchListDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdatePunchListCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PunchListManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetPunchListActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
