using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Contractors;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.ContractorView)]
public class ContractorsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateContractorDto> _createValidator;
    private readonly IValidator<UpdateContractorDto> _updateValidator;

    public ContractorsController(IMediator mediator, IValidator<CreateContractorDto> createValidator, IValidator<UpdateContractorDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var contractors = await _mediator.Send(new GetAllContractorsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", contractors);
        }

        return View(contractors);
    }

    [PermissionAuthorize(PermissionNames.ContractorManage)]
    public IActionResult Create()
    {
        return View(new CreateContractorDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ContractorManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateContractorDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }

        await _mediator.Send(new CreateContractorCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.ContractorManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var contractor = await _mediator.Send(new GetContractorByIdQuery(id));
        if (contractor is null)
        {
            return NotFound();
        }

        var dto = new UpdateContractorDto
        {
            Id = contractor.Id,
            ContractorCode = contractor.ContractorCode,
            Name = contractor.Name,
            ContactPerson = contractor.ContactPerson,
            Phone = contractor.Phone,
            Email = contractor.Email,
            Address = contractor.Address,
            LicenseNumber = contractor.LicenseNumber,
            Specialization = contractor.Specialization
        };

        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ContractorManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateContractorDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateContractorCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ContractorManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetContractorActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }
}
