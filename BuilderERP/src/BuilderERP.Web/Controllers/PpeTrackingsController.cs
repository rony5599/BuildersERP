using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.PpeTrackings;
using BuilderERP.Application.Features.Workers;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.PpeTrackingView)]
public class PpeTrackingsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreatePpeTrackingDto> _createValidator;
    private readonly IValidator<UpdatePpeTrackingDto> _updateValidator;

    public PpeTrackingsController(IMediator mediator, IValidator<CreatePpeTrackingDto> createValidator, IValidator<UpdatePpeTrackingDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllPpeTrackingsQuery(Page: page, PageSize: pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.PpeTrackingManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreatePpeTrackingDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PpeTrackingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePpeTrackingDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreatePpeTrackingCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.PpeTrackingManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetPpeTrackingByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdatePpeTrackingDto
        {
            Id = item.Id,
            WorkerId = item.WorkerId,
            PpeType = item.PpeType,
            IssueDate = item.IssueDate,
            ExpiryDate = item.ExpiryDate,
            Status = item.Status,
            Remarks = item.Remarks
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PpeTrackingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdatePpeTrackingDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdatePpeTrackingCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PpeTrackingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetPpeTrackingActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var workers = await _mediator.Send(new GetAllWorkersQuery(PageSize: int.MaxValue));
        ViewBag.Workers = new SelectList(workers.Items, "Id", "Name");
    }
}
