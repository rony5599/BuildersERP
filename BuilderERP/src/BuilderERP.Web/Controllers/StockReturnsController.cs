using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.StockReturns;
using BuilderERP.Application.Features.Warehouses;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.StockReturnView)]
public class StockReturnsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateStockReturnDto> _createValidator;
    private readonly IValidator<UpdateStockReturnDto> _updateValidator;

    public StockReturnsController(IMediator mediator, IValidator<CreateStockReturnDto> createValidator, IValidator<UpdateStockReturnDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(long? projectId, int page = 1, int pageSize = 25)
    {
        var returns = await _mediator.Send(new GetAllStockReturnsQuery(projectId, page, pageSize));
        ViewBag.SelectedProjectId = projectId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", returns);
        }

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);
        return View(returns);
    }

    [PermissionAuthorize(PermissionNames.StockReturnManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateStockReturnDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.StockReturnManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStockReturnDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateStockReturnCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.StockReturnManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var stockReturn = await _mediator.Send(new GetStockReturnByIdQuery(id));
        if (stockReturn is null)
        {
            return NotFound();
        }

        var dto = new UpdateStockReturnDto
        {
            Id = stockReturn.Id,
            ReturnNumber = stockReturn.ReturnNumber,
            ReturnDate = stockReturn.ReturnDate,
            Quantity = stockReturn.Quantity,
            Reason = stockReturn.Reason,
            MaterialId = stockReturn.MaterialId,
            WarehouseId = stockReturn.WarehouseId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.StockReturnManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateStockReturnDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateStockReturnCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.StockReturnManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetStockReturnActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var materials = await _mediator.Send(new GetAllMaterialsQuery(PageSize: int.MaxValue));
        ViewBag.Materials = materials.Items.Select(m => new SelectListItem
        {
            Value = m.Id.ToString(),
            Text = $"{m.MaterialCode} | {m.Name} | {m.CategoryName}"
        }).ToList();

        var warehouses = await _mediator.Send(new GetAllWarehousesQuery(PageSize: int.MaxValue));
        ViewBag.Warehouses = new SelectList(warehouses.Items, "Id", "Name");
    }
}
