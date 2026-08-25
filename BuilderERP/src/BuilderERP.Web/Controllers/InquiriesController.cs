using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Inquiries;
using BuilderERP.Application.Features.Leads;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.InquiryView)]
public class InquiriesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateInquiryDto> _createValidator;
    private readonly IValidator<UpdateInquiryDto> _updateValidator;

    public InquiriesController(IMediator mediator, IValidator<CreateInquiryDto> createValidator, IValidator<UpdateInquiryDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var inquiries = await _mediator.Send(new GetAllInquiriesQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", inquiries);
        }

        return View(inquiries);
    }

    [PermissionAuthorize(PermissionNames.InquiryManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateInquiryDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.InquiryManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInquiryDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateInquiryCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.InquiryManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var inquiry = await _mediator.Send(new GetInquiryByIdQuery(id));
        if (inquiry is null)
        {
            return NotFound();
        }

        var dto = new UpdateInquiryDto
        {
            Id = inquiry.Id,
            Message = inquiry.Message,
            InquiryDate = inquiry.InquiryDate,
            LeadId = inquiry.LeadId,
            PropertyUnitId = inquiry.PropertyUnitId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.InquiryManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateInquiryDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateInquiryCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.InquiryManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetInquiryActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var leads = await _mediator.Send(new GetAllLeadsQuery(PageSize: int.MaxValue));
        ViewBag.Leads = new SelectList(leads.Items, "Id", "Name");

        var units = await _mediator.Send(new GetAllPropertyUnitsQuery(PageSize: int.MaxValue));
        ViewBag.PropertyUnits = new SelectList(units.Items, "Id", "UnitNumber");
    }
}
