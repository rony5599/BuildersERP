using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Contractors;
using BuilderERP.Application.Features.Workers;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.WorkerView)]
public class WorkersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateWorkerDto> _createValidator;
    private readonly IValidator<UpdateWorkerDto> _updateValidator;

    public WorkersController(IMediator mediator, IValidator<CreateWorkerDto> createValidator, IValidator<UpdateWorkerDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var workers = await _mediator.Send(new GetAllWorkersQuery(Page: page, PageSize: pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", workers);
        }

        return View(workers);
    }

    [PermissionAuthorize(PermissionNames.WorkerManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateWorkerDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WorkerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWorkerDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateWorkerCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.WorkerManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var worker = await _mediator.Send(new GetWorkerByIdQuery(id));
        if (worker is null)
        {
            return NotFound();
        }

        var dto = new UpdateWorkerDto
        {
            Id = worker.Id,
            WorkerCode = worker.WorkerCode,
            Name = worker.Name,
            Phone = worker.Phone,
            Address = worker.Address,
            Trade = worker.Trade,
            DailyWageRate = worker.DailyWageRate,
            JoinDate = worker.JoinDate,
            ContractorId = worker.ContractorId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WorkerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateWorkerDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateWorkerCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WorkerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetWorkerActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var contractors = await _mediator.Send(new GetAllContractorsQuery(PageSize: int.MaxValue));
        ViewBag.Contractors = new SelectList(contractors.Items, "Id", "Name");

    }
}
