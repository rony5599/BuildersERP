using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.FollowUps;
using BuilderERP.Application.Features.Leads;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.FollowUpView)]
public class FollowUpsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateFollowUpDto> _createValidator;
    private readonly IValidator<UpdateFollowUpDto> _updateValidator;

    public FollowUpsController(IMediator mediator, IValidator<CreateFollowUpDto> createValidator, IValidator<UpdateFollowUpDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var followUps = await _mediator.Send(new GetAllFollowUpsQuery());
        return View(followUps);
    }

    [PermissionAuthorize(PermissionNames.FollowUpManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateFollowUpDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.FollowUpManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateFollowUpDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateFollowUpCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.FollowUpManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var followUp = await _mediator.Send(new GetFollowUpByIdQuery(id));
        if (followUp is null)
        {
            return NotFound();
        }

        var dto = new UpdateFollowUpDto
        {
            Id = followUp.Id,
            FollowUpDate = followUp.FollowUpDate,
            Notes = followUp.Notes,
            NextFollowUpDate = followUp.NextFollowUpDate,
            Outcome = followUp.Outcome,
            LeadId = followUp.LeadId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.FollowUpManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateFollowUpDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateFollowUpCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.FollowUpManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetFollowUpActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var leads = await _mediator.Send(new GetAllLeadsQuery());
        ViewBag.Leads = new SelectList(leads, "Id", "Name");
    }
}
