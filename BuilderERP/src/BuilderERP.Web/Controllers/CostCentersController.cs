using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.CostCenters;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.CostCenterView)]
public class CostCentersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateCostCenterDto> _createValidator;
    private readonly IValidator<UpdateCostCenterDto> _updateValidator;

    public CostCentersController(IMediator mediator, IValidator<CreateCostCenterDto> createValidator, IValidator<UpdateCostCenterDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var costCenters = await _mediator.Send(new GetAllCostCentersQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", costCenters);
        }

        return View(costCenters);
    }

    [PermissionAuthorize(PermissionNames.CostCenterManage)]
    public async Task<IActionResult> Create()
    {
        ViewBag.Projects = new SelectList((await _mediator.Send(new BuilderERP.Application.Features.Projects.GetAllProjectsQuery(PageSize: int.MaxValue))).Items, "Id", "Name");
        return View(new CreateCostCenterDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CostCenterManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCostCenterDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            ViewBag.Projects = new SelectList((await _mediator.Send(new BuilderERP.Application.Features.Projects.GetAllProjectsQuery(PageSize: int.MaxValue))).Items, "Id", "Name");
            return View(dto);
        }

        await _mediator.Send(new CreateCostCenterCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.CostCenterManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var costCenter = await _mediator.Send(new GetCostCenterByIdQuery(id));
        if (costCenter is null)
        {
            return NotFound();
        }

        var dto = new UpdateCostCenterDto
        {
            Id = costCenter.Id,
            Name = costCenter.Name,
            Code = costCenter.Code,
            ProjectId = costCenter.ProjectId
        };

        ViewBag.Projects = new SelectList((await _mediator.Send(new BuilderERP.Application.Features.Projects.GetAllProjectsQuery(PageSize: int.MaxValue))).Items, "Id", "Name");
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CostCenterManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateCostCenterDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            ViewBag.Projects = new SelectList((await _mediator.Send(new BuilderERP.Application.Features.Projects.GetAllProjectsQuery(PageSize: int.MaxValue))).Items, "Id", "Name");
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateCostCenterCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CostCenterManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetCostCenterActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }
}
