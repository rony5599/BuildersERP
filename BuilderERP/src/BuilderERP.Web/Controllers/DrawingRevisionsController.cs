using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Drawings;
using BuilderERP.Application.Features.DrawingRevisions;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.DrawingRevisionView)]
public class DrawingRevisionsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateDrawingRevisionDto> _createValidator;
    private readonly IValidator<UpdateDrawingRevisionDto> _updateValidator;
    private readonly IWebHostEnvironment _environment;

    public DrawingRevisionsController(
        IMediator mediator,
        IValidator<CreateDrawingRevisionDto> createValidator,
        IValidator<UpdateDrawingRevisionDto> updateValidator,
        IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _environment = environment;
    }

    public async Task<IActionResult> Index(Guid? drawingId)
    {
        var items = await _mediator.Send(new GetAllDrawingRevisionsQuery(drawingId));
        return View(items);
    }

    [PermissionAuthorize(PermissionNames.DrawingRevisionManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateDrawingRevisionDto { RevisedDate = DateTime.UtcNow });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DrawingRevisionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDrawingRevisionDto dto, IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            ModelState.AddModelError(nameof(file), "Please select a file to upload.");
        }
        else
        {
            var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "drawing-revisions");
            Directory.CreateDirectory(uploadsRoot);
            var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, safeFileName);
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            dto.FilePath = $"/uploads/drawing-revisions/{safeFileName}";
        }

        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!ModelState.IsValid || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateDrawingRevisionCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.DrawingRevisionManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetDrawingRevisionByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateDrawingRevisionDto
        {
            Id = item.Id,
            RevisionCode = item.RevisionCode,
            FilePath = item.FilePath,
            RevisedDate = item.RevisedDate,
            ChangeDescription = item.ChangeDescription,
            IsCurrent = item.IsCurrent,
            DrawingId = item.DrawingId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DrawingRevisionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateDrawingRevisionDto dto, IFormFile? file)
    {
        if (file is not null && file.Length > 0)
        {
            var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "drawing-revisions");
            Directory.CreateDirectory(uploadsRoot);
            var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, safeFileName);
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            dto.FilePath = $"/uploads/drawing-revisions/{safeFileName}";
        }
        else
        {
            var existing = await _mediator.Send(new GetDrawingRevisionByIdQuery(dto.Id));
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

        var success = await _mediator.Send(new UpdateDrawingRevisionCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DrawingRevisionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetDrawingRevisionActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var drawings = await _mediator.Send(new GetAllDrawingsQuery());
        ViewBag.Drawings = new SelectList(drawings, "Id", "DrawingNumber");
    }
}
