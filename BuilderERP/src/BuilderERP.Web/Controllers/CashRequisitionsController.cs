using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.CashRequisitions;
using BuilderERP.Application.Features.Departments;
using BuilderERP.Application.Features.Employees;
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

[PermissionAuthorize(PermissionNames.CashRequisitionView)]
public class CashRequisitionsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateCashRequisitionDto> _createValidator;
    private readonly IValidator<UpdateCashRequisitionDto> _updateValidator;

    public CashRequisitionsController(IMediator mediator, IValidator<CreateCashRequisitionDto> createValidator, IValidator<UpdateCashRequisitionDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var requisitions = await _mediator.Send(new GetAllCashRequisitionsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", requisitions);
        }

        return View(requisitions);
    }

    [PermissionAuthorize(PermissionNames.CashRequisitionManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateCashRequisitionDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashRequisitionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCashRequisitionDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateCashRequisitionCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.CashRequisitionManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var requisition = await _mediator.Send(new GetCashRequisitionByIdQuery(id));
        if (requisition is null)
        {
            return NotFound();
        }

        var dto = new UpdateCashRequisitionDto
        {
            Id = requisition.Id,
            RequisitionNumber = requisition.RequisitionNumber,
            RequestDate = requisition.RequestDate,
            RequiredByDate = requisition.RequiredByDate,
            Description = requisition.Description,
            Status = requisition.Status,
            RequesterEmployeeId = requisition.RequesterEmployeeId,
            PaymentMethod = requisition.PaymentMethod,
            DepartmentId = requisition.DepartmentId,
            ProjectId = requisition.ProjectId,
            Details = requisition.Details.Select(d => new CreateCashRequisitionDetailDto
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
    [PermissionAuthorize(PermissionNames.CashRequisitionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateCashRequisitionDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var result = await _mediator.Send(new UpdateCashRequisitionCommand(dto));
        if (result == UpdateCashRequisitionResult.NotFound)
        {
            return NotFound();
        }

        if (result == UpdateCashRequisitionResult.Locked)
        {
            ModelState.AddModelError(string.Empty, "This requisition has already been approved, rejected, or converted and cannot be edited.");
            await PopulateDropdownsAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CashRequisitionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetCashRequisitionActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var employees = await _mediator.Send(new GetAllEmployeesQuery(PageSize: int.MaxValue));
        ViewBag.Employees = new SelectList(employees.Items, "Id", "EmployeeName");

        var departments = await _mediator.Send(new GetAllDepartmentsQuery(PageSize: int.MaxValue));
        ViewBag.Departments = new SelectList(departments.Items, "Id", "Name");

        var materials = await _mediator.Send(new GetAllMaterialsQuery(PageSize: int.MaxValue));
        // Format materials with Code - Name | Category for multicolumn dropdown
        var formattedMaterials = materials.Items.Select(m => new SelectListItem
        {
            Value = m.Id.ToString(),
            Text = $"{m.MaterialCode} | {m.Name} | {m.CategoryName}"
        }).ToList();
        ViewBag.Materials = formattedMaterials;

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
