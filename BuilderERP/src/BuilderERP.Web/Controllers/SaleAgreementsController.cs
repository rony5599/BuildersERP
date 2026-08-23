using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Bookings;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.SaleAgreements;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SaleAgreementView)]
public class SaleAgreementsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateSaleAgreementDto> _createValidator;
    private readonly IValidator<UpdateSaleAgreementDto> _updateValidator;

    public SaleAgreementsController(IMediator mediator, IValidator<CreateSaleAgreementDto> createValidator, IValidator<UpdateSaleAgreementDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var agreements = await _mediator.Send(new GetAllSaleAgreementsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", agreements);
        }

        return View(agreements);
    }

    [PermissionAuthorize(PermissionNames.SaleAgreementManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateSaleAgreementDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SaleAgreementManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSaleAgreementDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateSaleAgreementCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.SaleAgreementManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var agreement = await _mediator.Send(new GetSaleAgreementByIdQuery(id));
        if (agreement is null)
        {
            return NotFound();
        }

        var dto = new UpdateSaleAgreementDto
        {
            Id = agreement.Id,
            AgreementNumber = agreement.AgreementNumber,
            AgreementDate = agreement.AgreementDate,
            TotalSalePrice = agreement.TotalSalePrice,
            Status = agreement.Status,
            BookingId = agreement.BookingId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SaleAgreementManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSaleAgreementDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateSaleAgreementCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SaleAgreementManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetSaleAgreementActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");

        var bookings = await _mediator.Send(new GetAllBookingsQuery(PageSize: int.MaxValue));
        ViewBag.Bookings = bookings.Items;
    }
}
