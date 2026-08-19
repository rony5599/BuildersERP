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
    private readonly IValidator<CreateBoqItemDto> _createValidator;
    private readonly IValidator<UpdateBoqItemDto> _updateValidator;

    public BoqItemsController(IMediator mediator, IValidator<CreateBoqItemDto> createValidator, IValidator<UpdateBoqItemDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(Guid? projectId, int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllBoqItemsQuery(projectId, page, pageSize));
        ViewBag.SelectedProjectId = projectId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateBoqItemDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBoqItemDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateBoqItemCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetBoqItemByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateBoqItemDto
        {
            Id = item.Id,
            ItemCode = item.ItemCode,
            Description = item.Description,
            UnitOfMeasure = item.UnitOfMeasure,
            Quantity = item.Quantity,
            Rate = item.Rate,
            Category = item.Category,
            ProjectId = item.ProjectId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateBoqItemDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateBoqItemCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BoqItemManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetBoqItemActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
