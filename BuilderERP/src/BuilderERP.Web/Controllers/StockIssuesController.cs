using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.StockIssues;
using BuilderERP.Application.Features.Warehouses;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.StockIssueView)]
public class StockIssuesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateStockIssueDto> _createValidator;
    private readonly IValidator<UpdateStockIssueDto> _updateValidator;

    public StockIssuesController(IMediator mediator, IValidator<CreateStockIssueDto> createValidator, IValidator<UpdateStockIssueDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var issues = await _mediator.Send(new GetAllStockIssuesQuery());
        return View(issues);
    }

    [PermissionAuthorize(PermissionNames.StockIssueManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateStockIssueDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.StockIssueManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStockIssueDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateStockIssueCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.StockIssueManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var issue = await _mediator.Send(new GetStockIssueByIdQuery(id));
        if (issue is null)
        {
            return NotFound();
        }

        var dto = new UpdateStockIssueDto
        {
            Id = issue.Id,
            IssueNumber = issue.IssueNumber,
            IssueDate = issue.IssueDate,
            Quantity = issue.Quantity,
            IssuedTo = issue.IssuedTo,
            ConsumedQuantity = issue.ConsumedQuantity,
            WastageQuantity = issue.WastageQuantity,
            WastageReason = issue.WastageReason,
            Remarks = issue.Remarks,
            MaterialId = issue.MaterialId,
            WarehouseId = issue.WarehouseId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.StockIssueManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateStockIssueDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateStockIssueCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.StockIssueManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetStockIssueActiveCommand(id, !isActive));
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
