using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.LandMutations;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.LandMutationView)]
public class LandMutationsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateLandMutationDto> _createValidator;
    private readonly IValidator<UpdateLandMutationDto> _updateValidator;

    public LandMutationsController(IMediator mediator, IValidator<CreateLandMutationDto> createValidator, IValidator<UpdateLandMutationDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllLandMutationsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.LandMutationManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateLandMutationDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LandMutationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLandMutationDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateLandMutationCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.LandMutationManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetLandMutationByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateLandMutationDto
        {
            Id = item.Id,
            MutationNumber = item.MutationNumber,
            ApplicantName = item.ApplicantName,
            KhatianNumber = item.KhatianNumber,
            DagNumber = item.DagNumber,
            MutationDate = item.MutationDate,
            Status = item.Status,
            Remarks = item.Remarks,
            ProjectId = item.ProjectId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LandMutationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateLandMutationDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateLandMutationCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LandMutationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetLandMutationActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
