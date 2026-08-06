using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.GoodsReceives;
using BuilderERP.Application.Features.PurchaseOrders;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.GoodsReceiveView)]
public class GoodsReceivesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateGoodsReceiveDto> _createValidator;
    private readonly IValidator<UpdateGoodsReceiveDto> _updateValidator;

    public GoodsReceivesController(IMediator mediator, IValidator<CreateGoodsReceiveDto> createValidator, IValidator<UpdateGoodsReceiveDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var receives = await _mediator.Send(new GetAllGoodsReceivesQuery());
        return View(receives);
    }

    [PermissionAuthorize(PermissionNames.GoodsReceiveManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateGoodsReceiveDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.GoodsReceiveManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGoodsReceiveDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateGoodsReceiveCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.GoodsReceiveManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var receive = await _mediator.Send(new GetGoodsReceiveByIdQuery(id));
        if (receive is null)
        {
            return NotFound();
        }

        var dto = new UpdateGoodsReceiveDto
        {
            Id = receive.Id,
            GrnNumber = receive.GrnNumber,
            ReceivedDate = receive.ReceivedDate,
            ReceivedAmount = receive.ReceivedAmount,
            Remarks = receive.Remarks,
            PurchaseOrderId = receive.PurchaseOrderId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.GoodsReceiveManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateGoodsReceiveDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateGoodsReceiveCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.GoodsReceiveManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetGoodsReceiveActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var orders = await _mediator.Send(new GetAllPurchaseOrdersQuery());
        ViewBag.PurchaseOrders = new SelectList(orders, "Id", "PONumber");
    }
}
