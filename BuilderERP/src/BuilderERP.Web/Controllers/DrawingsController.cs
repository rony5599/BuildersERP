using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Drawings;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using BuilderERP.Web.Storage;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.DrawingView)]
public class DrawingsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateDrawingDto> _createValidator;
    private readonly IValidator<UpdateDrawingDto> _updateValidator;
    private readonly IUploadsPathProvider _uploadsPathProvider;

    public DrawingsController(
        IMediator mediator,
        IValidator<CreateDrawingDto> createValidator,
        IValidator<UpdateDrawingDto> updateValidator,
        IUploadsPathProvider uploadsPathProvider)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _uploadsPathProvider = uploadsPathProvider;
    }

    public async Task<IActionResult> Index(long? projectId, int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllDrawingsQuery(projectId, page, pageSize));
        ViewBag.SelectedProjectId = projectId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.DrawingManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateDrawingDto { UploadedDate = DateTime.UtcNow });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DrawingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDrawingDto dto, IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            ModelState.AddModelError(nameof(file), "Please select a file to upload.");
        }
        else
        {
            var uploadsRoot = _uploadsPathProvider.GetPath("drawings");
            Directory.CreateDirectory(uploadsRoot);
            var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, safeFileName);
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            dto.FilePath = $"/uploads/drawings/{safeFileName}";
        }

        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!ModelState.IsValid || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateDrawingCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.DrawingManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var item = await _mediator.Send(new GetDrawingByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateDrawingDto
        {
            Id = item.Id,
            DrawingNumber = item.DrawingNumber,
            Title = item.Title,
            Discipline = item.Discipline,
            FilePath = item.FilePath,
            Status = item.Status,
            UploadedDate = item.UploadedDate,
            Remarks = item.Remarks,
            ProjectId = item.ProjectId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DrawingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateDrawingDto dto, IFormFile? file)
    {
        if (file is not null && file.Length > 0)
        {
            var uploadsRoot = _uploadsPathProvider.GetPath("drawings");
            Directory.CreateDirectory(uploadsRoot);
            var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, safeFileName);
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            dto.FilePath = $"/uploads/drawings/{safeFileName}";
        }
        else
        {
            var existing = await _mediator.Send(new GetDrawingByIdQuery(dto.Id));
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

        var success = await _mediator.Send(new UpdateDrawingCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DrawingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetDrawingActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
