using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Equipments;
using BuilderERP.Application.Features.FuelLogs;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.FuelLogView)]
public class FuelLogsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateFuelLogDto> _createValidator;
    private readonly IValidator<UpdateFuelLogDto> _updateValidator;

    public FuelLogsController(IMediator mediator, IValidator<CreateFuelLogDto> createValidator, IValidator<UpdateFuelLogDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _mediator.Send(new GetAllFuelLogsQuery());
        return View(logs);
    }

    [PermissionAuthorize(PermissionNames.FuelLogManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateFuelLogDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.FuelLogManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateFuelLogDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateFuelLogCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.FuelLogManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var log = await _mediator.Send(new GetFuelLogByIdQuery(id));
        if (log is null)
        {
            return NotFound();
        }

        var dto = new UpdateFuelLogDto
        {
            Id = log.Id,
            EquipmentId = log.EquipmentId,
            LogDate = log.LogDate,
            FuelQuantity = log.FuelQuantity,
            FuelCost = log.FuelCost,
            MeterReading = log.MeterReading,
            Remarks = log.Remarks
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.FuelLogManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateFuelLogDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateFuelLogCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.FuelLogManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetFuelLogActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var equipment = await _mediator.Send(new GetAllEquipmentQuery());
        ViewBag.Equipment = new SelectList(equipment, "Id", "Name");
    }
}
