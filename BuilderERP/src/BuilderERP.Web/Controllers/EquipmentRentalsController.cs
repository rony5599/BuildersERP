using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Equipments;
using BuilderERP.Application.Features.EquipmentRentals;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.Suppliers;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.EquipmentRentalView)]
public class EquipmentRentalsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateEquipmentRentalDto> _createValidator;
    private readonly IValidator<UpdateEquipmentRentalDto> _updateValidator;

    public EquipmentRentalsController(IMediator mediator, IValidator<CreateEquipmentRentalDto> createValidator, IValidator<UpdateEquipmentRentalDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var rentals = await _mediator.Send(new GetAllEquipmentRentalsQuery(Page: page, PageSize: pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", rentals);
        }

        return View(rentals);
    }

    [PermissionAuthorize(PermissionNames.EquipmentRentalManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateEquipmentRentalDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EquipmentRentalManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEquipmentRentalDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateEquipmentRentalCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.EquipmentRentalManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var rental = await _mediator.Send(new GetEquipmentRentalByIdQuery(id));
        if (rental is null)
        {
            return NotFound();
        }

        var dto = new UpdateEquipmentRentalDto
        {
            Id = rental.Id,
            EquipmentId = rental.EquipmentId,
            SupplierId = rental.SupplierId,
            ProjectId = rental.ProjectId,
            RentalStartDate = rental.RentalStartDate,
            RentalEndDate = rental.RentalEndDate,
            RatePerDay = rental.RatePerDay,
            Status = rental.Status
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EquipmentRentalManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateEquipmentRentalDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateEquipmentRentalCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EquipmentRentalManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetEquipmentRentalActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var equipment = await _mediator.Send(new GetAllEquipmentQuery(PageSize: int.MaxValue));
        ViewBag.Equipment = new SelectList(equipment.Items, "Id", "Name");

        var suppliers = await _mediator.Send(new GetAllSuppliersQuery(PageSize: int.MaxValue));
        ViewBag.Suppliers = new SelectList(suppliers.Items, "Id", "Name");

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
