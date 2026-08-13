using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.LegalAgreements;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.LegalAgreementView)]
public class LegalAgreementsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateLegalAgreementDto> _createValidator;
    private readonly IValidator<UpdateLegalAgreementDto> _updateValidator;

    public LegalAgreementsController(IMediator mediator, IValidator<CreateLegalAgreementDto> createValidator, IValidator<UpdateLegalAgreementDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _mediator.Send(new GetAllLegalAgreementsQuery());
        return View(items);
    }

    [PermissionAuthorize(PermissionNames.LegalAgreementManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateLegalAgreementDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LegalAgreementManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLegalAgreementDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateLegalAgreementCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.LegalAgreementManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetLegalAgreementByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateLegalAgreementDto
        {
            Id = item.Id,
            AgreementNumber = item.AgreementNumber,
            Title = item.Title,
            AgreementType = item.AgreementType,
            PartyName = item.PartyName,
            EffectiveDate = item.EffectiveDate,
            ExpiryDate = item.ExpiryDate,
            Status = item.Status,
            Remarks = item.Remarks,
            ProjectId = item.ProjectId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LegalAgreementManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateLegalAgreementDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateLegalAgreementCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LegalAgreementManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetLegalAgreementActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery());
        ViewBag.Projects = new SelectList(projects, "Id", "Name");
    }
}
