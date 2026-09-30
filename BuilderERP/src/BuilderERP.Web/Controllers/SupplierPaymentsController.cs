using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.SupplierPayments;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SupplierPaymentView)]
public class SupplierPaymentsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateSupplierPaymentDto> _validator;

    public SupplierPaymentsController(IMediator mediator, IValidator<CreateSupplierPaymentDto> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? search = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var payments = await _mediator.Send(new GetAllSupplierPaymentsQuery(page, pageSize, search, dateFrom, dateTo));

        ViewBag.Search = search;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", payments);
        }

        return View(payments);
    }

    [PermissionAuthorize(PermissionNames.SupplierPaymentManage)]
    public async Task<IActionResult> Create(long? poBillId = null, long? ewoBillId = null)
    {
        await PopulateAsync();
        return View(new CreateSupplierPaymentDto { PoBillId = poBillId, EwoBillId = ewoBillId });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SupplierPaymentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSupplierPaymentDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateAsync();
            return View(dto);
        }

        var result = await _mediator.Send(new CreateSupplierPaymentCommand(dto));
        if (result != CreateSupplierPaymentResult.Success)
        {
            ModelState.AddModelError(string.Empty, result == CreateSupplierPaymentResult.ExceedsOutstanding
                ? "The payment amount exceeds the outstanding balance of the selected bill."
                : "The selected bill is not approved or is inactive, so it cannot be paid.");
            await PopulateAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SupplierPaymentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetSupplierPaymentActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateAsync()
    {
        ViewBag.PayableBills = await _mediator.Send(new GetPayableBillsQuery());
    }
}
