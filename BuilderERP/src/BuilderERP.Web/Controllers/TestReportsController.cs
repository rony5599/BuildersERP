using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.TestReports;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.TestReportView)]
public class TestReportsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateTestReportDto> _createValidator;
    private readonly IValidator<UpdateTestReportDto> _updateValidator;
    private readonly IWebHostEnvironment _environment;

    public TestReportsController(
        IMediator mediator,
        IValidator<CreateTestReportDto> createValidator,
        IValidator<UpdateTestReportDto> updateValidator,
        IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _environment = environment;
    }

    public async Task<IActionResult> Index(Guid? projectId)
    {
        var items = await _mediator.Send(new GetAllTestReportsQuery(projectId));
        return View(items);
    }

    [PermissionAuthorize(PermissionNames.TestReportManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateTestReportDto { TestDate = DateTime.UtcNow });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.TestReportManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTestReportDto dto, IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            ModelState.AddModelError(nameof(file), "Please select a file to upload.");
        }
        else
        {
            var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "test-reports");
            Directory.CreateDirectory(uploadsRoot);
            var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, safeFileName);
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            dto.FilePath = $"/uploads/test-reports/{safeFileName}";
        }

        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!ModelState.IsValid || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateTestReportCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.TestReportManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetTestReportByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateTestReportDto
        {
            Id = item.Id,
            ReportNumber = item.ReportNumber,
            TestType = item.TestType,
            TestDate = item.TestDate,
            LabName = item.LabName,
            Result = item.Result,
            FilePath = item.FilePath,
            ProjectId = item.ProjectId,
            MaterialId = item.MaterialId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.TestReportManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateTestReportDto dto, IFormFile? file)
    {
        if (file is not null && file.Length > 0)
        {
            var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "test-reports");
            Directory.CreateDirectory(uploadsRoot);
            var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, safeFileName);
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            dto.FilePath = $"/uploads/test-reports/{safeFileName}";
        }
        else
        {
            var existing = await _mediator.Send(new GetTestReportByIdQuery(dto.Id));
            if (existing is null)
            {
                return NotFound();
            }

            dto.FilePath = existing.FilePath;
        }

        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateTestReportCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.TestReportManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetTestReportActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery());
        ViewBag.Projects = new SelectList(projects, "Id", "Name");

        var materials = await _mediator.Send(new GetAllMaterialsQuery());
        ViewBag.Materials = new SelectList(materials, "Id", "Name");
    }
}
