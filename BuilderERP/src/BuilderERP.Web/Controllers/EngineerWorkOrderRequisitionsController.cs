using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.EngineerWorkOrderRequisitions;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.EngineerWorkOrderRequisitionView)]
public class EngineerWorkOrderRequisitionsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateEngineerWorkOrderRequisitionDto> _createValidator;
    private readonly IValidator<UpdateEngineerWorkOrderRequisitionDto> _updateValidator;

    public EngineerWorkOrderRequisitionsController(IMediator mediator, IValidator<CreateEngineerWorkOrderRequisitionDto> createValidator, IValidator<UpdateEngineerWorkOrderRequisitionDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var requisitions = await _mediator.Send(new GetAllEngineerWorkOrderRequisitionsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", requisitions);
        }

        return View(requisitions);
    }

    [PermissionAuthorize(PermissionNames.EngineerWorkOrderRequisitionManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateEngineerWorkOrderRequisitionDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EngineerWorkOrderRequisitionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEngineerWorkOrderRequisitionDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateEngineerWorkOrderRequisitionCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.EngineerWorkOrderRequisitionManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var requisition = await _mediator.Send(new GetEngineerWorkOrderRequisitionByIdQuery(id));
        if (requisition is null)
        {
            return NotFound();
        }

        var dto = new UpdateEngineerWorkOrderRequisitionDto
        {
            Id = requisition.Id,
            RequisitionNumber = requisition.RequisitionNumber,
            RequestDate = requisition.RequestDate,
            RequiredByDate = requisition.RequiredByDate,
            Description = requisition.Description,
            Status = requisition.Status,
            ProjectId = requisition.ProjectId,
            Details = requisition.Details.Select(d => new CreateEngineerWorkOrderRequisitionDetailDto
            {
                MaterialId = d.MaterialId,
                Quantity = d.Quantity,
                UnitOfMeasure = d.UnitOfMeasure,
                EstimatedUnitPrice = d.EstimatedUnitPrice,
                Remarks = d.Remarks
            }).ToList()
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EngineerWorkOrderRequisitionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateEngineerWorkOrderRequisitionDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var result = await _mediator.Send(new UpdateEngineerWorkOrderRequisitionCommand(dto));
        if (result == UpdateEngineerWorkOrderRequisitionResult.NotFound)
        {
            return NotFound();
        }

        if (result == UpdateEngineerWorkOrderRequisitionResult.Locked)
        {
            ModelState.AddModelError(string.Empty, "This requisition has already been approved, rejected, or converted and cannot be edited.");
            await PopulateDropdownsAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EngineerWorkOrderRequisitionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetEngineerWorkOrderRequisitionActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var materials = await _mediator.Send(new GetAllMaterialsQuery(PageSize: int.MaxValue));
        ViewBag.Materials = new SelectList(materials.Items, "Id", "Name");

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
