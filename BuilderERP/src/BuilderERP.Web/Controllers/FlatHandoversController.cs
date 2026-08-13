using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.FlatHandovers;
using BuilderERP.Application.Features.Customers;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.FlatHandoverView)]
public class FlatHandoversController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateFlatHandoverDto> _createValidator;
    private readonly IValidator<UpdateFlatHandoverDto> _updateValidator;

    public FlatHandoversController(IMediator mediator, IValidator<CreateFlatHandoverDto> createValidator, IValidator<UpdateFlatHandoverDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _mediator.Send(new GetAllFlatHandoversQuery());
        return View(items);
    }

    [PermissionAuthorize(PermissionNames.FlatHandoverManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateFlatHandoverDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.FlatHandoverManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateFlatHandoverDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateFlatHandoverCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.FlatHandoverManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetFlatHandoverByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateFlatHandoverDto
        {
            Id = item.Id,
            HandoverNumber = item.HandoverNumber,
            HandoverDate = item.HandoverDate,
            KeyIssuedTo = item.KeyIssuedTo,
            Status = item.Status,
            Remarks = item.Remarks,
            PropertyUnitId = item.PropertyUnitId,
            CustomerId = item.CustomerId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.FlatHandoverManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateFlatHandoverDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateFlatHandoverCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.FlatHandoverManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetFlatHandoverActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var propertyunits = await _mediator.Send(new GetAllPropertyUnitsQuery());
        ViewBag.PropertyUnits = new SelectList(propertyunits, "Id", "UnitNumber");
        var customers = await _mediator.Send(new GetAllCustomersQuery());
        ViewBag.Customers = new SelectList(customers, "Id", "FullName");
    }
}
