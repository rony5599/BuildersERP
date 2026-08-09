using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Salaries;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SalaryView)]
public class SalariesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateSalaryDto> _createValidator;
    private readonly IValidator<UpdateSalaryDto> _updateValidator;

    public SalariesController(IMediator mediator, IValidator<CreateSalaryDto> createValidator, IValidator<UpdateSalaryDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var salaries = await _mediator.Send(new GetAllSalariesQuery());
        return View(salaries);
    }

    [PermissionAuthorize(PermissionNames.SalaryManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateSalaryDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SalaryManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSalaryDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateSalaryCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.SalaryManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var salary = await _mediator.Send(new GetSalaryByIdQuery(id));
        if (salary is null)
        {
            return NotFound();
        }

        var dto = new UpdateSalaryDto
        {
            Id = salary.Id,
            PeriodStart = salary.PeriodStart,
            PeriodEnd = salary.PeriodEnd,
            DaysWorked = salary.DaysWorked,
            BasicAmount = salary.BasicAmount,
            OvertimeAmount = salary.OvertimeAmount,
            DeductionAmount = salary.DeductionAmount,
            Status = salary.Status,
            PaymentDate = salary.PaymentDate,
            WorkerId = salary.WorkerId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SalaryManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSalaryDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateSalaryCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SalaryManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetSalaryActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var workers = await _mediator.Send(new BuilderERP.Application.Features.Workers.GetAllWorkersQuery());
        ViewBag.Workers = new SelectList(workers, "Id", "Name");
    }
}
