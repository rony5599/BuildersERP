using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Branches;
using BuilderERP.Application.Features.Companies;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.BranchView)]
public class BranchesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateBranchDto> _createValidator;
    private readonly IValidator<UpdateBranchDto> _updateValidator;

    public BranchesController(IMediator mediator, IValidator<CreateBranchDto> createValidator, IValidator<UpdateBranchDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var branches = await _mediator.Send(new GetAllBranchesQuery());
        return View(branches);
    }

    [PermissionAuthorize(PermissionNames.BranchManage)]
    public async Task<IActionResult> Create()
    {
        ViewBag.Companies = new SelectList(await _mediator.Send(new GetAllCompaniesQuery()), "Id", "Name");
        return View(new CreateBranchDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BranchManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBranchDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            ViewBag.Companies = new SelectList(await _mediator.Send(new GetAllCompaniesQuery()), "Id", "Name");
            return View(dto);
        }

        await _mediator.Send(new CreateBranchCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.BranchManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var branch = await _mediator.Send(new GetBranchByIdQuery(id));
        if (branch is null)
        {
            return NotFound();
        }

        var dto = new UpdateBranchDto
        {
            Id = branch.Id,
            Name = branch.Name,
            Code = branch.Code,
            Address = branch.Address,
            CompanyId = branch.CompanyId
        };

        ViewBag.Companies = new SelectList(await _mediator.Send(new GetAllCompaniesQuery()), "Id", "Name");
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BranchManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateBranchDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            ViewBag.Companies = new SelectList(await _mediator.Send(new GetAllCompaniesQuery()), "Id", "Name");
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateBranchCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BranchManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetBranchActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }
}
