using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.SnagItems;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SnagItemView)]
public class SnagItemsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateSnagItemDto> _createValidator;
    private readonly IValidator<UpdateSnagItemDto> _updateValidator;

    public SnagItemsController(IMediator mediator, IValidator<CreateSnagItemDto> createValidator, IValidator<UpdateSnagItemDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllSnagItemsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.SnagItemManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateSnagItemDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SnagItemManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSnagItemDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateSnagItemCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.SnagItemManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetSnagItemByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateSnagItemDto
        {
            Id = item.Id,
            SnagNumber = item.SnagNumber,
            Description = item.Description,
            Location = item.Location,
            Severity = item.Severity,
            Status = item.Status,
            ReportedDate = item.ReportedDate,
            ResolvedDate = item.ResolvedDate,
            Remarks = item.Remarks,
            PropertyUnitId = item.PropertyUnitId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SnagItemManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSnagItemDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateSnagItemCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SnagItemManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetSnagItemActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var propertyunits = await _mediator.Send(new GetAllPropertyUnitsQuery(PageSize: int.MaxValue));
        ViewBag.PropertyUnits = new SelectList(propertyunits.Items, "Id", "UnitNumber");
    }
}
