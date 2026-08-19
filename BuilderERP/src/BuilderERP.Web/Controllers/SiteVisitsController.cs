using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Leads;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Application.Features.SiteVisits;
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

[PermissionAuthorize(PermissionNames.SiteVisitView)]
public class SiteVisitsController : Controller
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IValidator<CreateSiteVisitDto> _createValidator;
    private readonly IValidator<UpdateSiteVisitDto> _updateValidator;

    public SiteVisitsController(
        IMediator mediator,
        UserManager<ApplicationUser> userManager,
        IValidator<CreateSiteVisitDto> createValidator,
        IValidator<UpdateSiteVisitDto> updateValidator)
    {
        _mediator = mediator;
        _userManager = userManager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var visits = await _mediator.Send(new GetAllSiteVisitsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", visits);
        }

        return View(visits);
    }

    [PermissionAuthorize(PermissionNames.SiteVisitManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateSiteVisitDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SiteVisitManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSiteVisitDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateSiteVisitCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.SiteVisitManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var visit = await _mediator.Send(new GetSiteVisitByIdQuery(id));
        if (visit is null)
        {
            return NotFound();
        }

        var dto = new UpdateSiteVisitDto
        {
            Id = visit.Id,
            VisitDate = visit.VisitDate,
            Status = visit.Status,
            Feedback = visit.Feedback,
            LeadId = visit.LeadId,
            PropertyUnitId = visit.PropertyUnitId,
            AssignedToUserId = visit.AssignedToUserId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SiteVisitManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSiteVisitDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateSiteVisitCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SiteVisitManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetSiteVisitActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var leads = await _mediator.Send(new GetAllLeadsQuery(PageSize: int.MaxValue));
        ViewBag.Leads = new SelectList(leads.Items, "Id", "Name");

        var units = await _mediator.Send(new GetAllPropertyUnitsQuery(PageSize: int.MaxValue));
        ViewBag.PropertyUnits = new SelectList(units.Items, "Id", "UnitNumber");

        var users = await _userManager.Users.ToListAsync();
        ViewBag.Users = new SelectList(users, "Id", "FullName");
    }
}
