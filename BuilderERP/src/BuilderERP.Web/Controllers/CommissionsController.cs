using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Bookings;
using BuilderERP.Application.Features.Brokers;
using BuilderERP.Application.Features.Commissions;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.CommissionView)]
public class CommissionsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateCommissionDto> _createValidator;
    private readonly IValidator<UpdateCommissionDto> _updateValidator;

    public CommissionsController(IMediator mediator, IValidator<CreateCommissionDto> createValidator, IValidator<UpdateCommissionDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var commissions = await _mediator.Send(new GetAllCommissionsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", commissions);
        }

        return View(commissions);
    }

    [PermissionAuthorize(PermissionNames.CommissionManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateCommissionDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CommissionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCommissionDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateCommissionCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.CommissionManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var commission = await _mediator.Send(new GetCommissionByIdQuery(id));
        if (commission is null)
        {
            return NotFound();
        }

        var dto = new UpdateCommissionDto
        {
            Id = commission.Id,
            CommissionRate = commission.CommissionRate,
            CommissionAmount = commission.CommissionAmount,
            Status = commission.Status,
            PaymentDate = commission.PaymentDate,
            Remarks = commission.Remarks,
            BrokerId = commission.BrokerId,
            BookingId = commission.BookingId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CommissionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateCommissionDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateCommissionCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CommissionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetCommissionActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var brokers = await _mediator.Send(new GetAllBrokersQuery(PageSize: int.MaxValue));
        ViewBag.Brokers = new SelectList(brokers.Items, "Id", "Name");

        var bookings = await _mediator.Send(new GetAllBookingsQuery(PageSize: int.MaxValue));
        var bookingOptions = bookings.Items.Select(b => new { b.Id, Display = $"{b.PropertyUnitNumber} - {b.CustomerName} ({b.BookingAmount:N0})" });
        ViewBag.Bookings = new SelectList(bookingOptions, "Id", "Display");
    }
}
