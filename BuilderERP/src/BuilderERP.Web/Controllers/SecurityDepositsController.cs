using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.SecurityDeposits;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SecurityDepositView)]
public class SecurityDepositsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateSecurityDepositDto> _createValidator;
    private readonly IValidator<UpdateSecurityDepositDto> _updateValidator;

    public SecurityDepositsController(IMediator mediator, IValidator<CreateSecurityDepositDto> createValidator, IValidator<UpdateSecurityDepositDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(Guid? contractorId, int page = 1, int pageSize = 25)
    {
        var deposits = await _mediator.Send(new GetAllSecurityDepositsQuery(contractorId, page, pageSize));
        ViewBag.SelectedContractorId = contractorId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", deposits);
        }

        return View(deposits);
    }

    [PermissionAuthorize(PermissionNames.SecurityDepositManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateSecurityDepositDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SecurityDepositManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSecurityDepositDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateSecurityDepositCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.SecurityDepositManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var deposit = await _mediator.Send(new GetSecurityDepositByIdQuery(id));
        if (deposit is null)
        {
            return NotFound();
        }

        var dto = new UpdateSecurityDepositDto
        {
            Id = deposit.Id,
            ContractorId = deposit.ContractorId,
            WorkOrderId = deposit.WorkOrderId,
            DepositAmount = deposit.DepositAmount,
            DepositDate = deposit.DepositDate,
            RefundDate = deposit.RefundDate,
            Status = deposit.Status
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SecurityDepositManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSecurityDepositDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateSecurityDepositCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SecurityDepositManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetSecurityDepositActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var contractors = await _mediator.Send(new BuilderERP.Application.Features.Contractors.GetAllContractorsQuery(PageSize: int.MaxValue));
        ViewBag.Contractors = new SelectList(contractors.Items, "Id", "Name");

        var workOrders = await _mediator.Send(new BuilderERP.Application.Features.WorkOrders.GetAllWorkOrdersQuery(PageSize: int.MaxValue));
        ViewBag.WorkOrders = new SelectList(workOrders.Items, "Id", "WorkOrderNumber");
    }
}
