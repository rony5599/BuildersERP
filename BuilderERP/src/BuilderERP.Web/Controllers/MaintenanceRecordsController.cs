using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.MaintenanceRecords;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.MaintenanceRecordView)]
public class MaintenanceRecordsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateMaintenanceRecordDto> _createValidator;
    private readonly IValidator<UpdateMaintenanceRecordDto> _updateValidator;

    public MaintenanceRecordsController(IMediator mediator, IValidator<CreateMaintenanceRecordDto> createValidator, IValidator<UpdateMaintenanceRecordDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var records = await _mediator.Send(new GetAllMaintenanceRecordsQuery(Page: page, PageSize: pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", records);
        }

        return View(records);
    }

    [PermissionAuthorize(PermissionNames.MaintenanceRecordManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateMaintenanceRecordDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaintenanceRecordManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMaintenanceRecordDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateMaintenanceRecordCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.MaintenanceRecordManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var record = await _mediator.Send(new GetMaintenanceRecordByIdQuery(id));
        if (record is null)
        {
            return NotFound();
        }

        var dto = new UpdateMaintenanceRecordDto
        {
            Id = record.Id,
            MaintenanceType = record.MaintenanceType,
            MaintenanceDate = record.MaintenanceDate,
            Description = record.Description,
            Cost = record.Cost,
            Status = record.Status,
            NextServiceDate = record.NextServiceDate,
            EquipmentId = record.EquipmentId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaintenanceRecordManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateMaintenanceRecordDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateMaintenanceRecordCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaintenanceRecordManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetMaintenanceRecordActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var equipment = await _mediator.Send(new BuilderERP.Application.Features.Equipments.GetAllEquipmentQuery(PageSize: int.MaxValue));
        ViewBag.Equipment = new SelectList(equipment.Items, "Id", "Name");
    }
}
