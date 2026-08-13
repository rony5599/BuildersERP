using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.UtilityBills;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.UtilityBillView)]
public class UtilityBillsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateUtilityBillDto> _createValidator;
    private readonly IValidator<UpdateUtilityBillDto> _updateValidator;

    public UtilityBillsController(IMediator mediator, IValidator<CreateUtilityBillDto> createValidator, IValidator<UpdateUtilityBillDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _mediator.Send(new GetAllUtilityBillsQuery());
        return View(items);
    }

    [PermissionAuthorize(PermissionNames.UtilityBillManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateUtilityBillDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.UtilityBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUtilityBillDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateUtilityBillCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.UtilityBillManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetUtilityBillByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateUtilityBillDto
        {
            Id = item.Id,
            BillNumber = item.BillNumber,
            UtilityType = item.UtilityType,
            BillingMonth = item.BillingMonth,
            Amount = item.Amount,
            DueDate = item.DueDate,
            PaidDate = item.PaidDate,
            Status = item.Status,
            Remarks = item.Remarks,
            PropertyUnitId = item.PropertyUnitId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.UtilityBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateUtilityBillDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateUtilityBillCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.UtilityBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetUtilityBillActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var propertyunits = await _mediator.Send(new GetAllPropertyUnitsQuery());
        ViewBag.PropertyUnits = new SelectList(propertyunits, "Id", "UnitNumber");
    }
}
