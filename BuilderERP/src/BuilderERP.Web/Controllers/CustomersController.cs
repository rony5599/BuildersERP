using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Customers;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.CustomerView)]
public class CustomersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateCustomerDto> _createValidator;
    private readonly IValidator<UpdateCustomerDto> _updateValidator;

    public CustomersController(IMediator mediator, IValidator<CreateCustomerDto> createValidator, IValidator<UpdateCustomerDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var customers = await _mediator.Send(new GetAllCustomersQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", customers);
        }

        return View(customers);
    }

    [PermissionAuthorize(PermissionNames.CustomerManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateCustomerDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CustomerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCustomerDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateCustomerCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.CustomerManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var customer = await _mediator.Send(new GetCustomerByIdQuery(id));
        if (customer is null)
        {
            return NotFound();
        }

        var dto = new UpdateCustomerDto
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            NIDNumber = customer.NIDNumber,
            KycStatus = customer.KycStatus,
            CompanyId = customer.CompanyId,
            LeadId = customer.LeadId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CustomerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateCustomerDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateCustomerCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CustomerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetCustomerActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var companies = await _mediator.Send(new BuilderERP.Application.Features.Companies.GetAllCompaniesQuery(PageSize: int.MaxValue));
        ViewBag.Companies = new SelectList(companies.Items, "Id", "Name");

        var leads = await _mediator.Send(new BuilderERP.Application.Features.Leads.GetAllLeadsQuery(PageSize: int.MaxValue));
        ViewBag.Leads = new SelectList(leads.Items, "Id", "Name");
    }
}
