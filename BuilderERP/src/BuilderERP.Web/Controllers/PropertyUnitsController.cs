using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.PropertyUnitView)]
public class PropertyUnitsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreatePropertyUnitDto> _createValidator;
    private readonly IValidator<UpdatePropertyUnitDto> _updateValidator;

    public PropertyUnitsController(IMediator mediator, IValidator<CreatePropertyUnitDto> createValidator, IValidator<UpdatePropertyUnitDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var units = await _mediator.Send(new GetAllPropertyUnitsQuery());
        return View(units);
    }

    [PermissionAuthorize(PermissionNames.PropertyUnitManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateFloorsAsync();
        return View(new CreatePropertyUnitDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PropertyUnitManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePropertyUnitDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateFloorsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreatePropertyUnitCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.PropertyUnitManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var unit = await _mediator.Send(new GetPropertyUnitByIdQuery(id));
        if (unit is null)
        {
            return NotFound();
        }

        var dto = new UpdatePropertyUnitDto
        {
            Id = unit.Id,
            UnitNumber = unit.UnitNumber,
            UnitType = unit.UnitType,
            Area = unit.Area,
            Price = unit.Price,
            BookingStatus = unit.BookingStatus,
            FloorId = unit.FloorId
        };

        await PopulateFloorsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PropertyUnitManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdatePropertyUnitDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateFloorsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdatePropertyUnitCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PropertyUnitManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetPropertyUnitActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateFloorsAsync()
    {
        var floors = await _mediator.Send(new BuilderERP.Application.Features.Floors.GetAllFloorsQuery());
        ViewBag.Floors = new SelectList(floors, "Id", "Name");
    }
}
