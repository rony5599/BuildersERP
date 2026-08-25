using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Equipments;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.EquipmentView)]
public class EquipmentController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateEquipmentDto> _createValidator;
    private readonly IValidator<UpdateEquipmentDto> _updateValidator;

    public EquipmentController(IMediator mediator, IValidator<CreateEquipmentDto> createValidator, IValidator<UpdateEquipmentDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var equipment = await _mediator.Send(new GetAllEquipmentQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", equipment);
        }

        return View(equipment);
    }

    [PermissionAuthorize(PermissionNames.EquipmentManage)]
    public IActionResult Create()
    {
        return View(new CreateEquipmentDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EquipmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEquipmentDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }

        await _mediator.Send(new CreateEquipmentCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.EquipmentManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var equipment = await _mediator.Send(new GetEquipmentByIdQuery(id));
        if (equipment is null)
        {
            return NotFound();
        }

        var dto = new UpdateEquipmentDto
        {
            Id = equipment.Id,
            EquipmentCode = equipment.EquipmentCode,
            Name = equipment.Name,
            Type = equipment.Type,
            Model = equipment.Model,
            RegistrationNumber = equipment.RegistrationNumber,
            PurchaseDate = equipment.PurchaseDate,
            Status = equipment.Status
        };

        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EquipmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateEquipmentDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateEquipmentCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EquipmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetEquipmentActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }
}
