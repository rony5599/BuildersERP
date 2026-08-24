using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.ItemCategories;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.ItemCategoryView)]
public class ItemCategoriesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateItemCategoryDto> _createValidator;
    private readonly IValidator<UpdateItemCategoryDto> _updateValidator;

    public ItemCategoriesController(IMediator mediator, IValidator<CreateItemCategoryDto> createValidator, IValidator<UpdateItemCategoryDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var categories = await _mediator.Send(new GetAllItemCategoriesQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", categories);
        }

        return View(categories);
    }

    [PermissionAuthorize(PermissionNames.ItemCategoryManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateItemCategoryDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ItemCategoryManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateItemCategoryDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateItemCategoryCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.ItemCategoryManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var category = await _mediator.Send(new GetItemCategoryByIdQuery(id));
        if (category is null)
        {
            return NotFound();
        }

        var dto = new UpdateItemCategoryDto
        {
            Id = category.Id,
            Code = category.Code,
            Name = category.Name,
            ParentCategoryId = category.ParentCategoryId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ItemCategoryManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateItemCategoryDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateItemCategoryCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ItemCategoryManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetItemCategoryActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var categories = await _mediator.Send(new GetAllItemCategoriesQuery(PageSize: int.MaxValue));
        ViewBag.ParentCategories = new SelectList(categories.Items, "Id", "Name");
    }
}
