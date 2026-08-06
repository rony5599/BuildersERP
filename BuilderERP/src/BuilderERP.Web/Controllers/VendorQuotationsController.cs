using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Rfqs;
using BuilderERP.Application.Features.VendorQuotations;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.VendorQuotationView)]
public class VendorQuotationsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateVendorQuotationDto> _createValidator;
    private readonly IValidator<UpdateVendorQuotationDto> _updateValidator;

    public VendorQuotationsController(IMediator mediator, IValidator<CreateVendorQuotationDto> createValidator, IValidator<UpdateVendorQuotationDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var quotations = await _mediator.Send(new GetAllVendorQuotationsQuery());
        return View(quotations);
    }

    [PermissionAuthorize(PermissionNames.VendorQuotationManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateVendorQuotationDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.VendorQuotationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVendorQuotationDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateVendorQuotationCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.VendorQuotationManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var quotation = await _mediator.Send(new GetVendorQuotationByIdQuery(id));
        if (quotation is null)
        {
            return NotFound();
        }

        var dto = new UpdateVendorQuotationDto
        {
            Id = quotation.Id,
            QuotationNumber = quotation.QuotationNumber,
            QuotationDate = quotation.QuotationDate,
            QuotedAmount = quotation.QuotedAmount,
            DeliveryDays = quotation.DeliveryDays,
            Status = quotation.Status,
            RfqId = quotation.RfqId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.VendorQuotationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateVendorQuotationDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateVendorQuotationCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.VendorQuotationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetVendorQuotationActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Compare(Guid requisitionId)
    {
        var quotations = await _mediator.Send(new GetVendorQuotationsByRequisitionIdQuery(requisitionId));
        ViewBag.PurchaseRequisitionId = requisitionId;
        return View(quotations);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.VendorQuotationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SelectWinner(Guid id, Guid requisitionId)
    {
        await _mediator.Send(new SelectWinnerVendorQuotationCommand(id));
        return RedirectToAction(nameof(Compare), new { requisitionId });
    }

    private async Task PopulateDropdownsAsync()
    {
        var rfqs = await _mediator.Send(new GetAllRfqsQuery());
        ViewBag.Rfqs = new SelectList(rfqs, "Id", "RfqNumber");
    }
}
