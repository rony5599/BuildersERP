using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Departments;
using BuilderERP.Application.Features.PurchaseRequisitions;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.PurchaseRequisitionView)]
public class PurchaseRequisitionsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreatePurchaseRequisitionDto> _createValidator;
    private readonly IValidator<UpdatePurchaseRequisitionDto> _updateValidator;

    public PurchaseRequisitionsController(IMediator mediator, IValidator<CreatePurchaseRequisitionDto> createValidator, IValidator<UpdatePurchaseRequisitionDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var requisitions = await _mediator.Send(new GetAllPurchaseRequisitionsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", requisitions);
        }

        return View(requisitions);
    }

    [PermissionAuthorize(PermissionNames.PurchaseRequisitionManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreatePurchaseRequisitionDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PurchaseRequisitionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePurchaseRequisitionDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreatePurchaseRequisitionCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.PurchaseRequisitionManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var requisition = await _mediator.Send(new GetPurchaseRequisitionByIdQuery(id));
        if (requisition is null)
        {
            return NotFound();
        }

        var dto = new UpdatePurchaseRequisitionDto
        {
            Id = requisition.Id,
            RequisitionNumber = requisition.RequisitionNumber,
            RequestDate = requisition.RequestDate,
            RequiredByDate = requisition.RequiredByDate,
            Description = requisition.Description,
            EstimatedAmount = requisition.EstimatedAmount,
            Status = requisition.Status,
            DepartmentId = requisition.DepartmentId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PurchaseRequisitionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdatePurchaseRequisitionDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdatePurchaseRequisitionCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PurchaseRequisitionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetPurchaseRequisitionActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var departments = await _mediator.Send(new GetAllDepartmentsQuery(PageSize: int.MaxValue));
        ViewBag.Departments = new SelectList(departments.Items, "Id", "Name");
    }
}
