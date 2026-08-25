using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.IncidentReports;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.IncidentReportView)]
public class IncidentReportsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateIncidentReportDto> _createValidator;
    private readonly IValidator<UpdateIncidentReportDto> _updateValidator;

    public IncidentReportsController(IMediator mediator, IValidator<CreateIncidentReportDto> createValidator, IValidator<UpdateIncidentReportDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var reports = await _mediator.Send(new GetAllIncidentReportsQuery(Page: page, PageSize: pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", reports);
        }

        return View(reports);
    }

    [PermissionAuthorize(PermissionNames.IncidentReportManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateIncidentReportDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.IncidentReportManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateIncidentReportDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateIncidentReportCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.IncidentReportManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var report = await _mediator.Send(new GetIncidentReportByIdQuery(id));
        if (report is null)
        {
            return NotFound();
        }

        var dto = new UpdateIncidentReportDto
        {
            Id = report.Id,
            ProjectId = report.ProjectId,
            IncidentDate = report.IncidentDate,
            ReportedBy = report.ReportedBy,
            Location = report.Location,
            Description = report.Description,
            Severity = report.Severity,
            InjuredPersonName = report.InjuredPersonName,
            Status = report.Status,
            CorrectiveAction = report.CorrectiveAction,
            ClosedDate = report.ClosedDate
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.IncidentReportManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateIncidentReportDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateIncidentReportCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.IncidentReportManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetIncidentReportActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
