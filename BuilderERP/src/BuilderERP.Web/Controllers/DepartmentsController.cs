using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Departments;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.DepartmentView)]
public class DepartmentsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateDepartmentDto> _createValidator;
    private readonly IValidator<UpdateDepartmentDto> _updateValidator;

    public DepartmentsController(IMediator mediator, IValidator<CreateDepartmentDto> createValidator, IValidator<UpdateDepartmentDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var departments = await _mediator.Send(new GetAllDepartmentsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", departments);
        }

        return View(departments);
    }

    [PermissionAuthorize(PermissionNames.DepartmentManage)]
    public async Task<IActionResult> Create()
    {
        ViewBag.Branches = new SelectList((await _mediator.Send(new BuilderERP.Application.Features.Branches.GetAllBranchesQuery(PageSize: int.MaxValue))).Items, "Id", "Name");
        return View(new CreateDepartmentDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DepartmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDepartmentDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            ViewBag.Branches = new SelectList((await _mediator.Send(new BuilderERP.Application.Features.Branches.GetAllBranchesQuery(PageSize: int.MaxValue))).Items, "Id", "Name");
            return View(dto);
        }

        await _mediator.Send(new CreateDepartmentCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.DepartmentManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var department = await _mediator.Send(new GetDepartmentByIdQuery(id));
        if (department is null)
        {
            return NotFound();
        }

        var dto = new UpdateDepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code,
            BranchId = department.BranchId
        };

        ViewBag.Branches = new SelectList((await _mediator.Send(new BuilderERP.Application.Features.Branches.GetAllBranchesQuery(PageSize: int.MaxValue))).Items, "Id", "Name");
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DepartmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateDepartmentDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            ViewBag.Branches = new SelectList((await _mediator.Send(new BuilderERP.Application.Features.Branches.GetAllBranchesQuery(PageSize: int.MaxValue))).Items, "Id", "Name");
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateDepartmentCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DepartmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetDepartmentActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }
}
