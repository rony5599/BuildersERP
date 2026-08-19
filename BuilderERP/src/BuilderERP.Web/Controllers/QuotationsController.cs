using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Customers;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Application.Features.Quotations;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.QuotationView)]
public class QuotationsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateQuotationDto> _createValidator;
    private readonly IValidator<UpdateQuotationDto> _updateValidator;

    public QuotationsController(IMediator mediator, IValidator<CreateQuotationDto> createValidator, IValidator<UpdateQuotationDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var quotations = await _mediator.Send(new GetAllQuotationsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", quotations);
        }

        return View(quotations);
    }

    [PermissionAuthorize(PermissionNames.QuotationManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateQuotationDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.QuotationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateQuotationDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateQuotationCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.QuotationManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var quotation = await _mediator.Send(new GetQuotationByIdQuery(id));
        if (quotation is null)
        {
            return NotFound();
        }

        var dto = new UpdateQuotationDto
        {
            Id = quotation.Id,
            QuotedPrice = quotation.QuotedPrice,
            ValidUntil = quotation.ValidUntil,
            Status = quotation.Status,
            CustomerId = quotation.CustomerId,
            PropertyUnitId = quotation.PropertyUnitId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.QuotationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateQuotationDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateQuotationCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.QuotationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetQuotationActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var customers = await _mediator.Send(new GetAllCustomersQuery(PageSize: int.MaxValue));
        ViewBag.Customers = new SelectList(customers.Items, "Id", "FullName");

        var units = await _mediator.Send(new GetAllPropertyUnitsQuery(PageSize: int.MaxValue));
        ViewBag.PropertyUnits = new SelectList(units.Items, "Id", "UnitNumber");
    }
}
