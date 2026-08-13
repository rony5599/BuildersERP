using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.ParkingSlots;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.ParkingSlotView)]
public class ParkingSlotsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateParkingSlotDto> _createValidator;
    private readonly IValidator<UpdateParkingSlotDto> _updateValidator;

    public ParkingSlotsController(IMediator mediator, IValidator<CreateParkingSlotDto> createValidator, IValidator<UpdateParkingSlotDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _mediator.Send(new GetAllParkingSlotsQuery());
        return View(items);
    }

    [PermissionAuthorize(PermissionNames.ParkingSlotManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateParkingSlotDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ParkingSlotManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateParkingSlotDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateParkingSlotCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.ParkingSlotManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetParkingSlotByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateParkingSlotDto
        {
            Id = item.Id,
            SlotNumber = item.SlotNumber,
            SlotType = item.SlotType,
            Status = item.Status,
            AllocatedTo = item.AllocatedTo,
            Remarks = item.Remarks,
            ProjectId = item.ProjectId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ParkingSlotManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateParkingSlotDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateParkingSlotCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ParkingSlotManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetParkingSlotActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery());
        ViewBag.Projects = new SelectList(projects, "Id", "Name");
    }
}
