using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.EngineerWorkOrderRequisitions;
using BuilderERP.Application.Features.EngineerWorkOrders;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Suppliers;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.EngineerWorkOrderView)]
public class EngineerWorkOrdersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateEngineerWorkOrderDto> _createValidator;
    private readonly IValidator<UpdateEngineerWorkOrderDto> _updateValidator;

    public EngineerWorkOrdersController(IMediator mediator, IValidator<CreateEngineerWorkOrderDto> createValidator, IValidator<UpdateEngineerWorkOrderDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var workOrders = await _mediator.Send(new GetAllEngineerWorkOrdersQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", workOrders);
        }

        return View(workOrders);
    }

    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateEngineerWorkOrderDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEngineerWorkOrderDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateEngineerWorkOrderCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var workOrder = await _mediator.Send(new GetEngineerWorkOrderByIdQuery(id));
        if (workOrder is null)
        {
            return NotFound();
        }

        var dto = new UpdateEngineerWorkOrderDto
        {
            Id = workOrder.Id,
            WorkOrderNo = workOrder.WorkOrderNo,
            EngineerWorkOrderRequisitionId = workOrder.EngineerWorkOrderRequisitionId,
            SupplierId = workOrder.SupplierId,
            TermsAndCondition = workOrder.TermsAndCondition,
            Status = workOrder.Status,
            Details = workOrder.Details.Select(d => new CreateEngineerWorkOrderDetailDto
            {
                MaterialId = d.MaterialId,
                UnitOfMeasure = d.UnitOfMeasure,
                Qty = d.Qty,
                Rate = d.Rate,
                Remarks = d.Remarks
            }).ToList()
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateEngineerWorkOrderDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var result = await _mediator.Send(new UpdateEngineerWorkOrderCommand(dto));
        if (result == UpdateEngineerWorkOrderResult.NotFound)
        {
            return NotFound();
        }

        if (result == UpdateEngineerWorkOrderResult.Locked)
        {
            ModelState.AddModelError(string.Empty, "This work order is no longer editable in its current status.");
            await PopulateDropdownsAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    public async Task<IActionResult> Revise(long id)
    {
        var workOrder = await _mediator.Send(new GetEngineerWorkOrderByIdQuery(id));
        if (workOrder is null)
        {
            return NotFound();
        }

        if (!workOrder.IsLatestRevision)
        {
            TempData["Error"] = "Only the latest revision of a work order can be revised.";
            return RedirectToAction(nameof(Index));
        }

        var dto = new CreateEngineerWorkOrderDto
        {
            EngineerWorkOrderRequisitionId = workOrder.EngineerWorkOrderRequisitionId,
            SupplierId = workOrder.SupplierId,
            TermsAndCondition = workOrder.TermsAndCondition,
            Status = Domain.Enums.EngineerWorkOrderStatus.Draft,
            Details = workOrder.Details.Select(d => new CreateEngineerWorkOrderDetailDto
            {
                MaterialId = d.MaterialId,
                UnitOfMeasure = d.UnitOfMeasure,
                Qty = d.Qty,
                Rate = d.Rate,
                Remarks = d.Remarks
            }).ToList()
        };

        await PopulateDropdownsAsync();
        ViewBag.PreviousWorkOrderId = id;
        ViewBag.IsRevision = true;
        return View("Create", dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Revise(long previousWorkOrderId, CreateEngineerWorkOrderDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            ViewBag.PreviousWorkOrderId = previousWorkOrderId;
            ViewBag.IsRevision = true;
            return View("Create", dto);
        }

        var result = await _mediator.Send(new CreateEngineerWorkOrderRevisionCommand(previousWorkOrderId, dto));

        if (result == CreateEngineerWorkOrderRevisionResult.NotFound)
        {
            return NotFound();
        }

        if (result == CreateEngineerWorkOrderRevisionResult.NotLatestRevision)
        {
            ModelState.AddModelError(string.Empty, "Only the latest revision of a work order can be revised.");
            await PopulateDropdownsAsync();
            ViewBag.PreviousWorkOrderId = previousWorkOrderId;
            ViewBag.IsRevision = true;
            return View("Create", dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [PermissionAuthorize(PermissionNames.EngineerWorkOrderManage)]
    public async Task<IActionResult> RequisitionItems(long requisitionId)
    {
        var requisition = await _mediator.Send(new GetEngineerWorkOrderRequisitionByIdQuery(requisitionId));
        if (requisition is null)
        {
            return NotFound();
        }

        var items = requisition.Details.Select(d => new
        {
            materialId = d.MaterialId,
            unitOfMeasure = (int)d.UnitOfMeasure,
            qty = d.Quantity,
            rate = d.EstimatedUnitPrice,
            remarks = d.Remarks
        });

        return Json(items);
    }

    private async Task PopulateDropdownsAsync()
    {
        var requisitions = await _mediator.Send(new GetAllEngineerWorkOrderRequisitionsQuery(PageSize: int.MaxValue));
        ViewBag.EngineerWorkOrderRequisitions = new SelectList(requisitions.Items, "Id", "RequisitionNumber");

        var suppliers = await _mediator.Send(new GetAllSuppliersQuery(PageSize: int.MaxValue));
        ViewBag.Suppliers = new SelectList(suppliers.Items, "Id", "Name");

        var materials = await _mediator.Send(new GetAllMaterialsQuery(PageSize: int.MaxValue));
        ViewBag.Materials = new SelectList(materials.Items, "Id", "Name");
    }
}
