using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.ItemCategories;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.MaterialView)]
public class MaterialsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateMaterialDto> _createValidator;
    private readonly IValidator<UpdateMaterialDto> _updateValidator;

    public MaterialsController(IMediator mediator, IValidator<CreateMaterialDto> createValidator, IValidator<UpdateMaterialDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(long? projectId, int page = 1, int pageSize = 25, string? search = null, long? categoryId = null, bool? isActive = null)
    {
        var materials = await _mediator.Send(new GetAllMaterialsQuery(projectId, page, pageSize, search, categoryId, isActive));
        ViewBag.SelectedProjectId = projectId;
        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;
        ViewBag.IsActive = isActive;

        var categories = await _mediator.Send(new GetAllItemCategoriesQuery(PageSize: int.MaxValue));
        ViewBag.Categories = new SelectList(categories.Items, "Id", "Name", categoryId);

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", materials);
        }

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);
        return View(materials);
    }

    [PermissionAuthorize(PermissionNames.MaterialManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateMaterialDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaterialManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMaterialDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateMaterialCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.MaterialManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var material = await _mediator.Send(new GetMaterialByIdQuery(id));
        if (material is null)
        {
            return NotFound();
        }

        var dto = new UpdateMaterialDto
        {
            Id = material.Id,
            MaterialCode = material.MaterialCode,
            Name = material.Name,
            Description = material.Description,
            UnitOfMeasure = material.UnitOfMeasure,
            ReorderLevel = material.ReorderLevel,
            Barcode = material.Barcode,
            CategoryId = material.CategoryId,
            Brand = material.Brand,
            PurchaseUnit = material.PurchaseUnit,
            UnitConversionFactor = material.UnitConversionFactor,
            MinStockLevel = material.MinStockLevel,
            MaxStockLevel = material.MaxStockLevel,
            StandardPurchasePrice = material.StandardPurchasePrice,
            VatPercent = material.VatPercent,
            TaxPercent = material.TaxPercent,
            DiscountPercent = material.DiscountPercent,
            IsBatchTracked = material.IsBatchTracked,
            IsSerialTracked = material.IsSerialTracked
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaterialManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateMaterialDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateMaterialCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaterialManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetMaterialActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var categories = await _mediator.Send(new GetAllItemCategoriesQuery(PageSize: int.MaxValue));
        ViewBag.Categories = new SelectList(categories.Items, "Id", "Name");
    }
}
