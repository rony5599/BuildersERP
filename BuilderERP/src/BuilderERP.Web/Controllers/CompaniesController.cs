using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Companies;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.CompanyView)]
public class CompaniesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateCompanyDto> _createValidator;
    private readonly IValidator<UpdateCompanyDto> _updateValidator;

    public CompaniesController(IMediator mediator, IValidator<CreateCompanyDto> createValidator, IValidator<UpdateCompanyDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var companies = await _mediator.Send(new GetAllCompaniesQuery());
        return View(companies);
    }

    [PermissionAuthorize(PermissionNames.CompanyManage)]
    public IActionResult Create()
    {
        return View(new CreateCompanyDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CompanyManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCompanyDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }

        await _mediator.Send(new CreateCompanyCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.CompanyManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var company = await _mediator.Send(new GetCompanyByIdQuery(id));
        if (company is null)
        {
            return NotFound();
        }

        var dto = new UpdateCompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            Code = company.Code,
            RegistrationNumber = company.RegistrationNumber,
            Address = company.Address,
            Phone = company.Phone,
            Email = company.Email
        };

        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CompanyManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateCompanyDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateCompanyCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CompanyManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetCompanyActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }
}
