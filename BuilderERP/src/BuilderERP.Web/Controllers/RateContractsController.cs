using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.RateContracts;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.RateContractView)]
public class RateContractsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateRateContractDto> _createValidator;
    private readonly IValidator<UpdateRateContractDto> _updateValidator;

    public RateContractsController(IMediator mediator, IValidator<CreateRateContractDto> createValidator, IValidator<UpdateRateContractDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var contracts = await _mediator.Send(new GetAllRateContractsQuery());
        return View(contracts);
    }

    [PermissionAuthorize(PermissionNames.RateContractManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateRateContractDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RateContractManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRateContractDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateRateContractCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.RateContractManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var contract = await _mediator.Send(new GetRateContractByIdQuery(id));
        if (contract is null)
        {
            return NotFound();
        }

        var dto = new UpdateRateContractDto
        {
            Id = contract.Id,
            ContractNumber = contract.ContractNumber,
            ItemDescription = contract.ItemDescription,
            UnitOfMeasure = contract.UnitOfMeasure,
            Rate = contract.Rate,
            EffectiveDate = contract.EffectiveDate,
            ExpiryDate = contract.ExpiryDate,
            ContractorId = contract.ContractorId,
            ProjectId = contract.ProjectId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RateContractManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateRateContractDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateRateContractCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RateContractManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetRateContractActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var contractors = await _mediator.Send(new BuilderERP.Application.Features.Contractors.GetAllContractorsQuery());
        ViewBag.Contractors = new SelectList(contractors, "Id", "Name");

        var projects = await _mediator.Send(new GetAllProjectsQuery());
        ViewBag.Projects = new SelectList(projects, "Id", "Name");
    }
}
