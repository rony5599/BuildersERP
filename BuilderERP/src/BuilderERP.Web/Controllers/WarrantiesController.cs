using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Warranties;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.WarrantyView)]
public class WarrantiesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateWarrantyDto> _createValidator;
    private readonly IValidator<UpdateWarrantyDto> _updateValidator;

    public WarrantiesController(IMediator mediator, IValidator<CreateWarrantyDto> createValidator, IValidator<UpdateWarrantyDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllWarrantiesQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.WarrantyManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateWarrantyDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WarrantyManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWarrantyDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateWarrantyCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.WarrantyManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetWarrantyByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateWarrantyDto
        {
            Id = item.Id,
            WarrantyNumber = item.WarrantyNumber,
            ItemCovered = item.ItemCovered,
            WarrantyType = item.WarrantyType,
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            Status = item.Status,
            Remarks = item.Remarks,
            PropertyUnitId = item.PropertyUnitId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WarrantyManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateWarrantyDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateWarrantyCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WarrantyManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetWarrantyActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var propertyunits = await _mediator.Send(new GetAllPropertyUnitsQuery(PageSize: int.MaxValue));
        ViewBag.PropertyUnits = new SelectList(propertyunits.Items, "Id", "UnitNumber");
    }
}
