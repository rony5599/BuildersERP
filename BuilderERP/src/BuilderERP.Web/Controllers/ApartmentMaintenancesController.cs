using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.ApartmentMaintenances;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.ApartmentMaintenanceView)]
public class ApartmentMaintenancesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateApartmentMaintenanceDto> _createValidator;
    private readonly IValidator<UpdateApartmentMaintenanceDto> _updateValidator;

    public ApartmentMaintenancesController(IMediator mediator, IValidator<CreateApartmentMaintenanceDto> createValidator, IValidator<UpdateApartmentMaintenanceDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _mediator.Send(new GetAllApartmentMaintenancesQuery());
        return View(items);
    }

    [PermissionAuthorize(PermissionNames.ApartmentMaintenanceManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateApartmentMaintenanceDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ApartmentMaintenanceManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateApartmentMaintenanceDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateApartmentMaintenanceCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.ApartmentMaintenanceManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetApartmentMaintenanceByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateApartmentMaintenanceDto
        {
            Id = item.Id,
            MaintenanceNumber = item.MaintenanceNumber,
            MaintenanceType = item.MaintenanceType,
            ScheduledDate = item.ScheduledDate,
            CompletedDate = item.CompletedDate,
            Cost = item.Cost,
            Status = item.Status,
            AssignedTo = item.AssignedTo,
            Remarks = item.Remarks,
            PropertyUnitId = item.PropertyUnitId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ApartmentMaintenanceManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateApartmentMaintenanceDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateApartmentMaintenanceCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ApartmentMaintenanceManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetApartmentMaintenanceActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var propertyunits = await _mediator.Send(new GetAllPropertyUnitsQuery());
        ViewBag.PropertyUnits = new SelectList(propertyunits, "Id", "UnitNumber");
    }
}
