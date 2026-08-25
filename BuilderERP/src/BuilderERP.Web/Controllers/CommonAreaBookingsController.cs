using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.CommonAreaBookings;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.CommonAreaBookingView)]
public class CommonAreaBookingsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateCommonAreaBookingDto> _createValidator;
    private readonly IValidator<UpdateCommonAreaBookingDto> _updateValidator;

    public CommonAreaBookingsController(IMediator mediator, IValidator<CreateCommonAreaBookingDto> createValidator, IValidator<UpdateCommonAreaBookingDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllCommonAreaBookingsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.CommonAreaBookingManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateCommonAreaBookingDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CommonAreaBookingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCommonAreaBookingDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateCommonAreaBookingCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.CommonAreaBookingManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var item = await _mediator.Send(new GetCommonAreaBookingByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateCommonAreaBookingDto
        {
            Id = item.Id,
            BookingNumber = item.BookingNumber,
            FacilityName = item.FacilityName,
            BookingDate = item.BookingDate,
            StartTime = item.StartTime,
            EndTime = item.EndTime,
            Fee = item.Fee,
            Status = item.Status,
            Remarks = item.Remarks,
            ProjectId = item.ProjectId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CommonAreaBookingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateCommonAreaBookingDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateCommonAreaBookingCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CommonAreaBookingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetCommonAreaBookingActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
