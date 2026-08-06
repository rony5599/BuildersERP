using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.StockTransfers;
using BuilderERP.Application.Features.Warehouses;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.StockTransferView)]
public class StockTransfersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateStockTransferDto> _createValidator;
    private readonly IValidator<UpdateStockTransferDto> _updateValidator;

    public StockTransfersController(IMediator mediator, IValidator<CreateStockTransferDto> createValidator, IValidator<UpdateStockTransferDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var transfers = await _mediator.Send(new GetAllStockTransfersQuery());
        return View(transfers);
    }

    [PermissionAuthorize(PermissionNames.StockTransferManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateStockTransferDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.StockTransferManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStockTransferDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateStockTransferCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.StockTransferManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var transfer = await _mediator.Send(new GetStockTransferByIdQuery(id));
        if (transfer is null)
        {
            return NotFound();
        }

        var dto = new UpdateStockTransferDto
        {
            Id = transfer.Id,
            TransferNumber = transfer.TransferNumber,
            TransferDate = transfer.TransferDate,
            Quantity = transfer.Quantity,
            Remarks = transfer.Remarks,
            MaterialId = transfer.MaterialId,
            FromWarehouseId = transfer.FromWarehouseId,
            ToWarehouseId = transfer.ToWarehouseId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.StockTransferManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateStockTransferDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateStockTransferCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.StockTransferManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetStockTransferActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var materials = await _mediator.Send(new GetAllMaterialsQuery());
        ViewBag.Materials = new SelectList(materials, "Id", "Name");

        var warehouses = await _mediator.Send(new GetAllWarehousesQuery());
        ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
    }
}
