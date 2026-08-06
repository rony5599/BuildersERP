using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.GoodsReceives;
using BuilderERP.Application.Features.PurchaseReturns;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.PurchaseReturnView)]
public class PurchaseReturnsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreatePurchaseReturnDto> _createValidator;
    private readonly IValidator<UpdatePurchaseReturnDto> _updateValidator;

    public PurchaseReturnsController(IMediator mediator, IValidator<CreatePurchaseReturnDto> createValidator, IValidator<UpdatePurchaseReturnDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var returns = await _mediator.Send(new GetAllPurchaseReturnsQuery());
        return View(returns);
    }

    [PermissionAuthorize(PermissionNames.PurchaseReturnManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreatePurchaseReturnDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PurchaseReturnManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePurchaseReturnDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreatePurchaseReturnCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.PurchaseReturnManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var purchaseReturn = await _mediator.Send(new GetPurchaseReturnByIdQuery(id));
        if (purchaseReturn is null)
        {
            return NotFound();
        }

        var dto = new UpdatePurchaseReturnDto
        {
            Id = purchaseReturn.Id,
            ReturnNumber = purchaseReturn.ReturnNumber,
            ReturnDate = purchaseReturn.ReturnDate,
            ReturnAmount = purchaseReturn.ReturnAmount,
            Reason = purchaseReturn.Reason,
            Status = purchaseReturn.Status,
            GoodsReceiveId = purchaseReturn.GoodsReceiveId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PurchaseReturnManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdatePurchaseReturnDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdatePurchaseReturnCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PurchaseReturnManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetPurchaseReturnActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var receives = await _mediator.Send(new GetAllGoodsReceivesQuery());
        ViewBag.GoodsReceives = new SelectList(receives, "Id", "GrnNumber");
    }
}
