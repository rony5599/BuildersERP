using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.ProjectView)]
public class ProjectsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateProjectDto> _createValidator;
    private readonly IValidator<UpdateProjectDto> _updateValidator;

    public ProjectsController(IMediator mediator, IValidator<CreateProjectDto> createValidator, IValidator<UpdateProjectDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", projects);
        }

        return View(projects);
    }

    [PermissionAuthorize(PermissionNames.ProjectManage)]
    public async Task<IActionResult> Create()
    {
        ViewBag.Branches = new SelectList((await _mediator.Send(new BuilderERP.Application.Features.Branches.GetAllBranchesQuery(PageSize: int.MaxValue))).Items, "Id", "Name");
        return View(new CreateProjectDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ProjectManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProjectDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            ViewBag.Branches = new SelectList((await _mediator.Send(new BuilderERP.Application.Features.Branches.GetAllBranchesQuery(PageSize: int.MaxValue))).Items, "Id", "Name");
            return View(dto);
        }

        await _mediator.Send(new CreateProjectCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.ProjectManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var project = await _mediator.Send(new GetProjectByIdQuery(id));
        if (project is null)
        {
            return NotFound();
        }

        var dto = new UpdateProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Code = project.Code,
            Location = project.Location,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            BranchId = project.BranchId
        };

        ViewBag.Branches = new SelectList((await _mediator.Send(new BuilderERP.Application.Features.Branches.GetAllBranchesQuery(PageSize: int.MaxValue))).Items, "Id", "Name");
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ProjectManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateProjectDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            ViewBag.Branches = new SelectList((await _mediator.Send(new BuilderERP.Application.Features.Branches.GetAllBranchesQuery(PageSize: int.MaxValue))).Items, "Id", "Name");
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateProjectCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.ProjectManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetProjectActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }
}
