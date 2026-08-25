using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.CollectionTargets;
using BuilderERP.Domain.Entities;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.CollectionTargetView)]
public class CollectionTargetsController : Controller
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IValidator<CreateCollectionTargetDto> _createValidator;
    private readonly IValidator<UpdateCollectionTargetDto> _updateValidator;

    public CollectionTargetsController(
        IMediator mediator,
        UserManager<ApplicationUser> userManager,
        IValidator<CreateCollectionTargetDto> createValidator,
        IValidator<UpdateCollectionTargetDto> updateValidator)
    {
        _mediator = mediator;
        _userManager = userManager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var targets = await _mediator.Send(new GetAllCollectionTargetsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", targets);
        }

        return View(targets);
    }

    [PermissionAuthorize(PermissionNames.CollectionTargetManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateCollectionTargetDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CollectionTargetManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCollectionTargetDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateCollectionTargetCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.CollectionTargetManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var target = await _mediator.Send(new GetCollectionTargetByIdQuery(id));
        if (target is null)
        {
            return NotFound();
        }

        var dto = new UpdateCollectionTargetDto
        {
            Id = target.Id,
            Year = target.Year,
            Month = target.Month,
            TargetAmount = target.TargetAmount,
            CollectionOfficerId = target.CollectionOfficerId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CollectionTargetManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateCollectionTargetDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateCollectionTargetCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.CollectionTargetManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetCollectionTargetActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        ViewBag.CollectionOfficers = new SelectList(users, "Id", "FullName");
    }
}
