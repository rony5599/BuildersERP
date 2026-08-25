using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Contractors;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.WorkOrders;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.WorkOrderView)]
public class WorkOrdersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateWorkOrderDto> _createValidator;
    private readonly IValidator<UpdateWorkOrderDto> _updateValidator;

    public WorkOrdersController(IMediator mediator, IValidator<CreateWorkOrderDto> createValidator, IValidator<UpdateWorkOrderDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var workOrders = await _mediator.Send(new GetAllWorkOrdersQuery(Page: page, PageSize: pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", workOrders);
        }

        return View(workOrders);
    }

    [PermissionAuthorize(PermissionNames.WorkOrderManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateWorkOrderDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WorkOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWorkOrderDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateWorkOrderCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.WorkOrderManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var workOrder = await _mediator.Send(new GetWorkOrderByIdQuery(id));
        if (workOrder is null)
        {
            return NotFound();
        }

        var dto = new UpdateWorkOrderDto
        {
            Id = workOrder.Id,
            WorkOrderNumber = workOrder.WorkOrderNumber,
            Description = workOrder.Description,
            OrderDate = workOrder.OrderDate,
            CompletionDate = workOrder.CompletionDate,
            Amount = workOrder.Amount,
            Status = workOrder.Status,
            ContractorId = workOrder.ContractorId,
            ProjectId = workOrder.ProjectId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WorkOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateWorkOrderDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateWorkOrderCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.WorkOrderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetWorkOrderActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var contractors = await _mediator.Send(new GetAllContractorsQuery(PageSize: int.MaxValue));
        ViewBag.Contractors = new SelectList(contractors.Items, "Id", "Name");

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
