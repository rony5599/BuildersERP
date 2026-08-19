using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.PurchaseRequisitions;
using BuilderERP.Application.Features.Rfqs;
using BuilderERP.Application.Features.Suppliers;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.RfqView)]
public class RfqsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateRfqDto> _createValidator;
    private readonly IValidator<UpdateRfqDto> _updateValidator;

    public RfqsController(IMediator mediator, IValidator<CreateRfqDto> createValidator, IValidator<UpdateRfqDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var rfqs = await _mediator.Send(new GetAllRfqsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", rfqs);
        }

        return View(rfqs);
    }

    [PermissionAuthorize(PermissionNames.RfqManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateRfqDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RfqManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRfqDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateRfqCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.RfqManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var rfq = await _mediator.Send(new GetRfqByIdQuery(id));
        if (rfq is null)
        {
            return NotFound();
        }

        var dto = new UpdateRfqDto
        {
            Id = rfq.Id,
            RfqNumber = rfq.RfqNumber,
            IssueDate = rfq.IssueDate,
            ClosingDate = rfq.ClosingDate,
            Status = rfq.Status,
            PurchaseRequisitionId = rfq.PurchaseRequisitionId,
            SupplierId = rfq.SupplierId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RfqManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateRfqDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateRfqCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RfqManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetRfqActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var requisitions = await _mediator.Send(new GetAllPurchaseRequisitionsQuery(PageSize: int.MaxValue));
        ViewBag.PurchaseRequisitions = new SelectList(requisitions.Items, "Id", "RequisitionNumber");

        var suppliers = await _mediator.Send(new GetAllSuppliersQuery(PageSize: int.MaxValue));
        ViewBag.Suppliers = new SelectList(suppliers.Items, "Id", "Name");
    }
}
