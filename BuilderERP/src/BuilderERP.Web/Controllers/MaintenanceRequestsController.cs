using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.MaintenanceRequests;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.MaintenanceRequestView)]
public class MaintenanceRequestsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateMaintenanceRequestDto> _createValidator;
    private readonly IValidator<UpdateMaintenanceRequestDto> _updateValidator;

    public MaintenanceRequestsController(IMediator mediator, IValidator<CreateMaintenanceRequestDto> createValidator, IValidator<UpdateMaintenanceRequestDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _mediator.Send(new GetAllMaintenanceRequestsQuery());
        return View(items);
    }

    [PermissionAuthorize(PermissionNames.MaintenanceRequestManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateMaintenanceRequestDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaintenanceRequestManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMaintenanceRequestDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateMaintenanceRequestCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.MaintenanceRequestManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetMaintenanceRequestByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateMaintenanceRequestDto
        {
            Id = item.Id,
            RequestNumber = item.RequestNumber,
            RequestType = item.RequestType,
            Description = item.Description,
            Priority = item.Priority,
            Status = item.Status,
            RequestDate = item.RequestDate,
            ResolvedDate = item.ResolvedDate,
            AssignedTo = item.AssignedTo,
            Remarks = item.Remarks,
            PropertyUnitId = item.PropertyUnitId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaintenanceRequestManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateMaintenanceRequestDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateMaintenanceRequestCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.MaintenanceRequestManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetMaintenanceRequestActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var propertyunits = await _mediator.Send(new GetAllPropertyUnitsQuery());
        ViewBag.PropertyUnits = new SelectList(propertyunits, "Id", "UnitNumber");
    }
}
