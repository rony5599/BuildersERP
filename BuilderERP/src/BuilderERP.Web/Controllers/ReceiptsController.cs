using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Installments;
using BuilderERP.Application.Features.Receipts;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.ReceiptView)]
public class ReceiptsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateReceiptDto> _createValidator;
    private readonly IValidator<UpdateReceiptDto> _updateValidator;

    public ReceiptsController(IMediator mediator, IValidator<CreateReceiptDto> createValidator, IValidator<UpdateReceiptDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var receipts = await _mediator.Send(new GetAllReceiptsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", receipts);
        }

        return View(receipts);
    }

    [PermissionAuthorize(PermissionNames.ReceiptManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateReceiptDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ReceiptManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateReceiptDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateReceiptCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.ReceiptManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var receipt = await _mediator.Send(new GetReceiptByIdQuery(id));
        if (receipt is null)
        {
            return NotFound();
        }

        var dto = new UpdateReceiptDto
        {
            Id = receipt.Id,
            ReceiptNumber = receipt.ReceiptNumber,
            PaymentDate = receipt.PaymentDate,
            AmountPaid = receipt.AmountPaid,
            PaymentMethod = receipt.PaymentMethod,
            Notes = receipt.Notes,
            InstallmentId = receipt.InstallmentId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ReceiptManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateReceiptDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateReceiptCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ReceiptManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetReceiptActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var installments = await _mediator.Send(new GetAllInstallmentsQuery(PageSize: int.MaxValue));
        ViewBag.Installments = installments.Items;
    }
}
