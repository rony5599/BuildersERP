using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.ContractorLedgers;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.ContractorLedgerView)]
public class ContractorLedgersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateContractorLedgerDto> _createValidator;
    private readonly IValidator<UpdateContractorLedgerDto> _updateValidator;

    public ContractorLedgersController(IMediator mediator, IValidator<CreateContractorLedgerDto> createValidator, IValidator<UpdateContractorLedgerDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(Guid? contractorId)
    {
        var ledgers = await _mediator.Send(new GetAllContractorLedgersQuery(contractorId));
        return View(ledgers);
    }

    [PermissionAuthorize(PermissionNames.ContractorLedgerManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateContractorLedgerDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ContractorLedgerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateContractorLedgerDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateContractorLedgerCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.ContractorLedgerManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var ledger = await _mediator.Send(new GetContractorLedgerByIdQuery(id));
        if (ledger is null)
        {
            return NotFound();
        }

        var dto = new UpdateContractorLedgerDto
        {
            Id = ledger.Id,
            ContractorId = ledger.ContractorId,
            TransactionDate = ledger.TransactionDate,
            Description = ledger.Description,
            DebitAmount = ledger.DebitAmount,
            CreditAmount = ledger.CreditAmount
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ContractorLedgerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateContractorLedgerDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateContractorLedgerCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ContractorLedgerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetContractorLedgerActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var contractors = await _mediator.Send(new BuilderERP.Application.Features.Contractors.GetAllContractorsQuery());
        ViewBag.Contractors = new SelectList(contractors, "Id", "Name");
    }
}
