using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.CustomerCommunications;
using BuilderERP.Application.Features.Customers;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.CustomerCommunicationView)]
public class CustomerCommunicationsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateCustomerCommunicationDto> _createValidator;
    private readonly IValidator<UpdateCustomerCommunicationDto> _updateValidator;

    public CustomerCommunicationsController(IMediator mediator, IValidator<CreateCustomerCommunicationDto> createValidator, IValidator<UpdateCustomerCommunicationDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var communications = await _mediator.Send(new GetAllCustomerCommunicationsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", communications);
        }

        return View(communications);
    }

    [PermissionAuthorize(PermissionNames.CustomerCommunicationManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateCustomerCommunicationDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CustomerCommunicationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCustomerCommunicationDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateCustomerCommunicationCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.CustomerCommunicationManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var communication = await _mediator.Send(new GetCustomerCommunicationByIdQuery(id));
        if (communication is null)
        {
            return NotFound();
        }

        var dto = new UpdateCustomerCommunicationDto
        {
            Id = communication.Id,
            CommunicationDate = communication.CommunicationDate,
            Type = communication.Type,
            Subject = communication.Subject,
            Notes = communication.Notes,
            CustomerId = communication.CustomerId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CustomerCommunicationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateCustomerCommunicationDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateCustomerCommunicationCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CustomerCommunicationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetCustomerCommunicationActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var customers = await _mediator.Send(new GetAllCustomersQuery(PageSize: int.MaxValue));
        ViewBag.Customers = new SelectList(customers.Items, "Id", "FullName");
    }
}
