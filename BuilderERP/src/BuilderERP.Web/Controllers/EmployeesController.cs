using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Branches;
using BuilderERP.Application.Features.Departments;
using BuilderERP.Application.Features.Designations;
using BuilderERP.Application.Features.Employees;
using BuilderERP.Domain.Entities;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.EmployeeView)]
public class EmployeesController : Controller
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IValidator<CreateEmployeeDto> _createValidator;
    private readonly IValidator<UpdateEmployeeDto> _updateValidator;

    public EmployeesController(
        IMediator mediator,
        UserManager<ApplicationUser> userManager,
        IValidator<CreateEmployeeDto> createValidator,
        IValidator<UpdateEmployeeDto> updateValidator)
    {
        _mediator = mediator;
        _userManager = userManager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var employees = await _mediator.Send(new GetAllEmployeesQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", employees);
        }

        return View(employees);
    }

    [PermissionAuthorize(PermissionNames.EmployeeManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateEmployeeDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EmployeeManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEmployeeDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateEmployeeCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.EmployeeManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var employee = await _mediator.Send(new GetEmployeeByIdQuery(id));
        if (employee is null)
        {
            return NotFound();
        }

        var dto = new UpdateEmployeeDto
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            EmployeeName = employee.EmployeeName,
            MobileNo = employee.MobileNo,
            Email = employee.Email,
            DepartmentId = employee.DepartmentId,
            DesignationId = employee.DesignationId,
            BranchId = employee.BranchId,
            ReportingToId = employee.ReportingToId,
            UserId = employee.UserId
        };

        await PopulateDropdownsAsync(id);
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EmployeeManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateEmployeeDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync(dto.Id);
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateEmployeeCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.EmployeeManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetEmployeeActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(long? excludeEmployeeId = null)
    {
        var departments = await _mediator.Send(new GetAllDepartmentsQuery(PageSize: int.MaxValue));
        ViewBag.Departments = new SelectList(departments.Items, "Id", "Name");

        var designations = await _mediator.Send(new GetAllDesignationsQuery(PageSize: int.MaxValue));
        ViewBag.Designations = new SelectList(designations.Items, "Id", "Name");

        var branches = await _mediator.Send(new GetAllBranchesQuery(PageSize: int.MaxValue));
        ViewBag.Branches = new SelectList(branches.Items, "Id", "Name");

        var employees = await _mediator.Send(new GetAllEmployeesQuery(PageSize: int.MaxValue));
        var reportingToOptions = employees.Items.Where(e => e.Id != excludeEmployeeId);
        ViewBag.ReportingToOptions = new SelectList(reportingToOptions, "Id", "EmployeeName");

        ViewBag.Users = new SelectList(_userManager.Users.OrderBy(u => u.Email).ToList(), "Id", "Email");
    }
}
