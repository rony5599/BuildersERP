using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Installments;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.InstallmentView)]
public class InstallmentsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateInstallmentDto> _createValidator;
    private readonly IValidator<UpdateInstallmentDto> _updateValidator;

    public InstallmentsController(IMediator mediator, IValidator<CreateInstallmentDto> createValidator, IValidator<UpdateInstallmentDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var installments = await _mediator.Send(new GetAllInstallmentsQuery());
        return View(installments);
    }

    [PermissionAuthorize(PermissionNames.InstallmentManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateInstallmentDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.InstallmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInstallmentDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateInstallmentCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.InstallmentManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var installment = await _mediator.Send(new GetInstallmentByIdQuery(id));
        if (installment is null)
        {
            return NotFound();
        }

        var dto = new UpdateInstallmentDto
        {
            Id = installment.Id,
            InstallmentNumber = installment.InstallmentNumber,
            DueDate = installment.DueDate,
            DueAmount = installment.DueAmount,
            PenaltyAmount = installment.PenaltyAmount,
            PaidAmount = installment.PaidAmount,
            Status = installment.Status,
            InstallmentPlanId = installment.InstallmentPlanId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.InstallmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateInstallmentDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateInstallmentCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.InstallmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetInstallmentActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.InstallmentManage)]
    public async Task<IActionResult> Reschedule(Guid id)
    {
        var installment = await _mediator.Send(new GetInstallmentByIdQuery(id));
        if (installment is null)
        {
            return NotFound();
        }

        return View(installment);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.InstallmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reschedule(Guid id, DateTime newDueDate, string reason)
    {
        await _mediator.Send(new RescheduleInstallmentCommand(id, newDueDate, reason));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var plans = await _mediator.Send(new BuilderERP.Application.Features.InstallmentPlans.GetAllInstallmentPlansQuery());
        ViewBag.InstallmentPlans = new SelectList(plans, "Id", "TotalAmount");
    }
}
