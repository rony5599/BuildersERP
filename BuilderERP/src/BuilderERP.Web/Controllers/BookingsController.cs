using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Bookings;
using BuilderERP.Application.Features.Brokers;
using BuilderERP.Application.Features.Customers;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Domain.Entities;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.BookingView)]
public class BookingsController : Controller
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IValidator<CreateBookingDto> _createValidator;
    private readonly IValidator<UpdateBookingDto> _updateValidator;

    public BookingsController(IMediator mediator, UserManager<ApplicationUser> userManager, IValidator<CreateBookingDto> createValidator, IValidator<UpdateBookingDto> updateValidator)
    {
        _mediator = mediator;
        _userManager = userManager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var bookings = await _mediator.Send(new GetAllBookingsQuery());
        return View(bookings);
    }

    [PermissionAuthorize(PermissionNames.BookingManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateBookingDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BookingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookingDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateBookingCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.BookingManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var booking = await _mediator.Send(new GetBookingByIdQuery(id));
        if (booking is null)
        {
            return NotFound();
        }

        var dto = new UpdateBookingDto
        {
            Id = booking.Id,
            BookingDate = booking.BookingDate,
            BookingAmount = booking.BookingAmount,
            Status = booking.Status,
            CancellationReason = booking.CancellationReason,
            CustomerId = booking.CustomerId,
            PropertyUnitId = booking.PropertyUnitId,
            BrokerId = booking.BrokerId,
            CollectionOfficerId = booking.CollectionOfficerId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BookingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateBookingDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateBookingCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BookingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetBookingActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var customers = await _mediator.Send(new GetAllCustomersQuery());
        ViewBag.Customers = new SelectList(customers, "Id", "FullName");

        var units = await _mediator.Send(new GetAllPropertyUnitsQuery());
        ViewBag.PropertyUnits = new SelectList(units, "Id", "UnitNumber");

        var brokers = await _mediator.Send(new GetAllBrokersQuery());
        ViewBag.Brokers = new SelectList(brokers, "Id", "Name");

        var users = await _userManager.Users.ToListAsync();
        ViewBag.CollectionOfficers = new SelectList(users, "Id", "FullName");
    }
}
