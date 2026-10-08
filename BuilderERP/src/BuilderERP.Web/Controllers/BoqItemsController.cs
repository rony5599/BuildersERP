using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.BoqItems;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.BoqItemView)]
public class BoqItemsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<UpdateBoqDto> _updateValidator;
    private readonly IValidator<CreateBoqDto> _createBoqValidator;

    public BoqItemsController(IMediator mediator, IValidator<UpdateBoqDto> updateValidator, IValidator<CreateBoqDto> createBoqValidator)
    {
        _mediator = mediator;
        _updateValidator = updateValidator;
        _createBoqValidator = createBoqValidator;
    }

    public async Task<IActionResult> Index(long? projectId, string? boqName, int page = 1, int pageSize = 25)
    {
        pageSize = pageSize is 10 or 25 or 50 or 100 ? pageSize : 25;
        var items = await _mediator.Send(new GetAllBoqsQuery(projectId, boqName, page, pageSize));
        ViewBag.SelectedProjectId = projectId;
        ViewBag.SelectedBoqName = boqName?.Trim();
        ViewBag.SelectedPageSize = pageSize;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);
        return View(items);
    }

    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateBoqDto { VersionNumber = 1, Items = new List<CreateBoqItemDto> { new() } });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBoqDto dto)
    {
        var validationResult = await _createBoqValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateBoqCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var dto = await _mediator.Send(new GetBoqForEditQuery(id));
        if (dto is null) return NotFound();
        await PopulateDropdownsAsync();
        ViewBag.IsEdit = true;
        return View("Create", dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateBoqDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            ViewBag.IsEdit = true;
            return View("Create", dto);
        }

        var success = await _mediator.Send(new UpdateBoqCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetBoqItemActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
        ViewBag.WorkGroups = await _mediator.Send(new GetWorkGroupOptionsQuery());
    }

}
