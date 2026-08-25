using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Leads;
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

[PermissionAuthorize(PermissionNames.LeadView)]
public class LeadsController : Controller
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IValidator<CreateLeadDto> _createValidator;
    private readonly IValidator<UpdateLeadDto> _updateValidator;

    public LeadsController(
        IMediator mediator,
        UserManager<ApplicationUser> userManager,
        IValidator<CreateLeadDto> createValidator,
        IValidator<UpdateLeadDto> updateValidator)
    {
        _mediator = mediator;
        _userManager = userManager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var leads = await _mediator.Send(new GetAllLeadsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", leads);
        }

        return View(leads);
    }

    [PermissionAuthorize(PermissionNames.LeadManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateUsersAsync();
        return View(new CreateLeadDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LeadManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLeadDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateUsersAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateLeadCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.LeadManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var lead = await _mediator.Send(new GetLeadByIdQuery(id));
        if (lead is null)
        {
            return NotFound();
        }

        var dto = new UpdateLeadDto
        {
            Id = lead.Id,
            Name = lead.Name,
            Phone = lead.Phone,
            Email = lead.Email,
            Source = lead.Source,
            Status = lead.Status,
            Notes = lead.Notes,
            AssignedToUserId = lead.AssignedToUserId
        };

        await PopulateUsersAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LeadManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateLeadDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateUsersAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateLeadCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LeadManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetLeadActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        ViewBag.Users = new SelectList(users, "Id", "FullName");
    }
}
