using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Designations;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.DesignationView)]
public class DesignationsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateDesignationDto> _createValidator;
    private readonly IValidator<UpdateDesignationDto> _updateValidator;

    public DesignationsController(IMediator mediator, IValidator<CreateDesignationDto> createValidator, IValidator<UpdateDesignationDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var designations = await _mediator.Send(new GetAllDesignationsQuery(page, pageSize));
        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", designations);
        }
        return View(designations);
    }

    [PermissionAuthorize(PermissionNames.DesignationManage)]
    public IActionResult Create()
    {
        return View(new CreateDesignationDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DesignationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDesignationDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }
        await _mediator.Send(new CreateDesignationCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.DesignationManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var designation = await _mediator.Send(new GetDesignationByIdQuery(id));
        if (designation is null)
        {
            return NotFound();
        }

        var dto = new UpdateDesignationDto { Id = designation.Id, Name = designation.Name, Code = designation.Code };
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DesignationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateDesignationDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }
        var success = await _mediator.Send(new UpdateDesignationCommand(dto));
        if (!success)
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DesignationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetDesignationActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }
}
