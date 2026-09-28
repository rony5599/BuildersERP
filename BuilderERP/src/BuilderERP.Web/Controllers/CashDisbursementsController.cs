using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.CashDisbursements;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.CashDisbursementView)]
public class CashDisbursementsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateCashDisbursementDto> _validator;

    public CashDisbursementsController(IMediator mediator, IValidator<CreateCashDisbursementDto> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? search = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var items = await _mediator.Send(new GetAllCashDisbursementsQuery(page, pageSize, search, dateFrom, dateTo));

        ViewBag.Search = search;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.CashDisbursementManage)]
    public async Task<IActionResult> Create(long? cashRequisitionId = null)
    {
        await PopulateAsync();
        return View(new CreateCashDisbursementDto { CashRequisitionId = cashRequisitionId ?? 0 });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashDisbursementManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCashDisbursementDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateAsync();
            return View(dto);
        }

        var result = await _mediator.Send(new CreateCashDisbursementCommand(dto));
        if (result != CreateCashDisbursementResult.Success)
        {
            ModelState.AddModelError(string.Empty, "The selected requisition is not approved or is inactive, so cash cannot be issued against it.");
            await PopulateAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashDisbursementManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetCashDisbursementActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateAsync()
    {
        ViewBag.Requisitions = await _mediator.Send(new GetDisbursableRequisitionsQuery());
    }
}
