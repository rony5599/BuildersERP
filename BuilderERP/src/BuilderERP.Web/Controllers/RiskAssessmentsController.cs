using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.RiskAssessments;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.RiskAssessmentView)]
public class RiskAssessmentsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateRiskAssessmentDto> _createValidator;
    private readonly IValidator<UpdateRiskAssessmentDto> _updateValidator;

    public RiskAssessmentsController(IMediator mediator, IValidator<CreateRiskAssessmentDto> createValidator, IValidator<UpdateRiskAssessmentDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var assessments = await _mediator.Send(new GetAllRiskAssessmentsQuery(Page: page, PageSize: pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", assessments);
        }

        return View(assessments);
    }

    [PermissionAuthorize(PermissionNames.RiskAssessmentManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateRiskAssessmentDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RiskAssessmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRiskAssessmentDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateRiskAssessmentCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.RiskAssessmentManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var assessment = await _mediator.Send(new GetRiskAssessmentByIdQuery(id));
        if (assessment is null)
        {
            return NotFound();
        }

        var dto = new UpdateRiskAssessmentDto
        {
            Id = assessment.Id,
            ProjectId = assessment.ProjectId,
            AssessmentDate = assessment.AssessmentDate,
            AssessedBy = assessment.AssessedBy,
            HazardDescription = assessment.HazardDescription,
            RiskLevel = assessment.RiskLevel,
            MitigationMeasures = assessment.MitigationMeasures,
            ReviewDate = assessment.ReviewDate
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RiskAssessmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateRiskAssessmentDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateRiskAssessmentCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RiskAssessmentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetRiskAssessmentActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
