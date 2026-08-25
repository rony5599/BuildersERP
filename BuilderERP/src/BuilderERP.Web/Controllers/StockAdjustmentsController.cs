using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.StockAdjustments;
using BuilderERP.Application.Features.Warehouses;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.StockAdjustmentView)]
public class StockAdjustmentsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateStockAdjustmentDto> _createValidator;
    private readonly IValidator<UpdateStockAdjustmentDto> _updateValidator;

    public StockAdjustmentsController(IMediator mediator, IValidator<CreateStockAdjustmentDto> createValidator, IValidator<UpdateStockAdjustmentDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(long? projectId, int page = 1, int pageSize = 25)
    {
        var adjustments = await _mediator.Send(new GetAllStockAdjustmentsQuery(projectId, page, pageSize));
        ViewBag.SelectedProjectId = projectId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", adjustments);
        }

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);
        return View(adjustments);
    }

    [PermissionAuthorize(PermissionNames.StockAdjustmentManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateStockAdjustmentDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.StockAdjustmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStockAdjustmentDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateStockAdjustmentCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.StockAdjustmentManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var adjustment = await _mediator.Send(new GetStockAdjustmentByIdQuery(id));
        if (adjustment is null)
        {
            return NotFound();
        }

        var dto = new UpdateStockAdjustmentDto
        {
            Id = adjustment.Id,
            AdjustmentNumber = adjustment.AdjustmentNumber,
            AdjustmentDate = adjustment.AdjustmentDate,
            QuantityDelta = adjustment.QuantityDelta,
            Reason = adjustment.Reason,
            MaterialId = adjustment.MaterialId,
            WarehouseId = adjustment.WarehouseId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.StockAdjustmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateStockAdjustmentDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateStockAdjustmentCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.StockAdjustmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetStockAdjustmentActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var materials = await _mediator.Send(new GetAllMaterialsQuery(PageSize: int.MaxValue));
        ViewBag.Materials = new SelectList(materials.Items, "Id", "Name");

        var warehouses = await _mediator.Send(new GetAllWarehousesQuery(PageSize: int.MaxValue));
        ViewBag.Warehouses = new SelectList(warehouses.Items, "Id", "Name");
    }
}
