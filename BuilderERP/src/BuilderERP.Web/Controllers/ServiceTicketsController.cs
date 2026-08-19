using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.ServiceTickets;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.ServiceTicketView)]
public class ServiceTicketsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateServiceTicketDto> _createValidator;
    private readonly IValidator<UpdateServiceTicketDto> _updateValidator;

    public ServiceTicketsController(IMediator mediator, IValidator<CreateServiceTicketDto> createValidator, IValidator<UpdateServiceTicketDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllServiceTicketsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.ServiceTicketManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateServiceTicketDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ServiceTicketManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateServiceTicketDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateServiceTicketCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.ServiceTicketManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetServiceTicketByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateServiceTicketDto
        {
            Id = item.Id,
            TicketNumber = item.TicketNumber,
            Subject = item.Subject,
            Description = item.Description,
            Category = item.Category,
            Priority = item.Priority,
            Status = item.Status,
            RaisedDate = item.RaisedDate,
            ClosedDate = item.ClosedDate,
            AssignedTo = item.AssignedTo,
            Remarks = item.Remarks,
            PropertyUnitId = item.PropertyUnitId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ServiceTicketManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateServiceTicketDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateServiceTicketCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ServiceTicketManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetServiceTicketActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var propertyunits = await _mediator.Send(new GetAllPropertyUnitsQuery(PageSize: int.MaxValue));
        ViewBag.PropertyUnits = new SelectList(propertyunits.Items, "Id", "UnitNumber");
    }
}
