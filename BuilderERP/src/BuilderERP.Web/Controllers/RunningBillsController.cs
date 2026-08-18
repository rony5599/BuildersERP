using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.RunningBills;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.RunningBillView)]
public class RunningBillsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateRunningBillDto> _createValidator;
    private readonly IValidator<UpdateRunningBillDto> _updateValidator;
    private readonly IValidator<CertifyRunningBillDto> _certifyValidator;

    public RunningBillsController(IMediator mediator, IValidator<CreateRunningBillDto> createValidator, IValidator<UpdateRunningBillDto> updateValidator, IValidator<CertifyRunningBillDto> certifyValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _certifyValidator = certifyValidator;
    }

    public async Task<IActionResult> Index()
    {
        var bills = await _mediator.Send(new GetAllRunningBillsQuery());
        return View(bills);
    }

    [PermissionAuthorize(PermissionNames.RunningBillManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateRunningBillDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RunningBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRunningBillDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateRunningBillCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.RunningBillManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var bill = await _mediator.Send(new GetRunningBillByIdQuery(id));
        if (bill is null)
        {
            return NotFound();
        }

        var dto = new UpdateRunningBillDto
        {
            Id = bill.Id,
            BillNumber = bill.BillNumber,
            BillDate = bill.BillDate,
            WorkDoneAmount = bill.WorkDoneAmount,
            PreviousBillAmount = bill.PreviousBillAmount,
            DeductionAmount = bill.DeductionAmount,
            Status = bill.Status,
            WorkOrderId = bill.WorkOrderId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RunningBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateRunningBillDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateRunningBillCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RunningBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetRunningBillActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.RunningBillManage)]
    public async Task<IActionResult> Certify(Guid id)
    {
        var bill = await _mediator.Send(new GetRunningBillByIdQuery(id));
        if (bill is null)
        {
            return NotFound();
        }

        var dto = new CertifyRunningBillDto
        {
            Id = bill.Id,
            CertifiedBy = User.Identity?.Name ?? string.Empty
        };

        ViewBag.RunningBill = bill;
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RunningBillManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Certify(CertifyRunningBillDto dto)
    {
        var validationResult = await _certifyValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            var bill = await _mediator.Send(new GetRunningBillByIdQuery(dto.Id));
            ViewBag.RunningBill = bill;
            return View(dto);
        }

        var success = await _mediator.Send(new CertifyRunningBillCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var workOrders = await _mediator.Send(new BuilderERP.Application.Features.WorkOrders.GetAllWorkOrdersQuery());
        ViewBag.WorkOrders = new SelectList(workOrders, "Id", "WorkOrderNumber");
    }
}
