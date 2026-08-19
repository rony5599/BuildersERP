using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.LegalCases;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.LegalCaseView)]
public class LegalCasesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateLegalCaseDto> _createValidator;
    private readonly IValidator<UpdateLegalCaseDto> _updateValidator;

    public LegalCasesController(IMediator mediator, IValidator<CreateLegalCaseDto> createValidator, IValidator<UpdateLegalCaseDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllLegalCasesQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.LegalCaseManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateLegalCaseDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LegalCaseManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLegalCaseDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateLegalCaseCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.LegalCaseManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetLegalCaseByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateLegalCaseDto
        {
            Id = item.Id,
            CaseNumber = item.CaseNumber,
            CaseTitle = item.CaseTitle,
            CourtName = item.CourtName,
            CaseType = item.CaseType,
            FilingDate = item.FilingDate,
            Status = item.Status,
            OpposingParty = item.OpposingParty,
            LawyerName = item.LawyerName,
            NextHearingDate = item.NextHearingDate,
            Remarks = item.Remarks,
            ProjectId = item.ProjectId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LegalCaseManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateLegalCaseDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateLegalCaseCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LegalCaseManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetLegalCaseActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
