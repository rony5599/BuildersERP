using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.PerformanceEvaluations;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.PerformanceEvaluationView)]
public class PerformanceEvaluationsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreatePerformanceEvaluationDto> _createValidator;
    private readonly IValidator<UpdatePerformanceEvaluationDto> _updateValidator;

    public PerformanceEvaluationsController(IMediator mediator, IValidator<CreatePerformanceEvaluationDto> createValidator, IValidator<UpdatePerformanceEvaluationDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(long? contractorId, int page = 1, int pageSize = 25)
    {
        var evaluations = await _mediator.Send(new GetAllPerformanceEvaluationsQuery(contractorId, page, pageSize));
        ViewBag.SelectedContractorId = contractorId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", evaluations);
        }

        return View(evaluations);
    }

    [PermissionAuthorize(PermissionNames.PerformanceEvaluationManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreatePerformanceEvaluationDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PerformanceEvaluationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePerformanceEvaluationDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreatePerformanceEvaluationCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.PerformanceEvaluationManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var evaluation = await _mediator.Send(new GetPerformanceEvaluationByIdQuery(id));
        if (evaluation is null)
        {
            return NotFound();
        }

        var dto = new UpdatePerformanceEvaluationDto
        {
            Id = evaluation.Id,
            ContractorId = evaluation.ContractorId,
            ProjectId = evaluation.ProjectId,
            EvaluationDate = evaluation.EvaluationDate,
            QualityScore = evaluation.QualityScore,
            TimelinessScore = evaluation.TimelinessScore,
            SafetyScore = evaluation.SafetyScore,
            Remarks = evaluation.Remarks
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PerformanceEvaluationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdatePerformanceEvaluationDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdatePerformanceEvaluationCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PerformanceEvaluationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetPerformanceEvaluationActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var contractors = await _mediator.Send(new BuilderERP.Application.Features.Contractors.GetAllContractorsQuery(PageSize: int.MaxValue));
        ViewBag.Contractors = new SelectList(contractors.Items, "Id", "Name");

        var projects = await _mediator.Send(new BuilderERP.Application.Features.Projects.GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
