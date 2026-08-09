using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.DailyProgresses;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.SitePhotos;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SitePhotoView)]
public class SitePhotosController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateSitePhotoDto> _createValidator;
    private readonly IValidator<UpdateSitePhotoDto> _updateValidator;
    private readonly IWebHostEnvironment _environment;

    public SitePhotosController(
        IMediator mediator,
        IValidator<CreateSitePhotoDto> createValidator,
        IValidator<UpdateSitePhotoDto> updateValidator,
        IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _environment = environment;
    }

    public async Task<IActionResult> Index(Guid? projectId)
    {
        var items = await _mediator.Send(new GetAllSitePhotosQuery(projectId));
        return View(items);
    }

    [PermissionAuthorize(PermissionNames.SitePhotoManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateSitePhotoDto { TakenDate = DateTime.UtcNow });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SitePhotoManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSitePhotoDto dto, IFormFile? photo)
    {
        if (photo is null || photo.Length == 0)
        {
            ModelState.AddModelError(nameof(photo), "Please select a photo to upload.");
        }
        else
        {
            dto.FilePath = await SavePhotoAsync(photo);
        }

        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!ModelState.IsValid || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateSitePhotoCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.SitePhotoManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetSitePhotoByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateSitePhotoDto
        {
            Id = item.Id,
            FilePath = item.FilePath,
            Caption = item.Caption,
            TakenDate = item.TakenDate,
            ProjectId = item.ProjectId,
            DailyProgressId = item.DailyProgressId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SitePhotoManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSitePhotoDto dto, IFormFile? photo)
    {
        if (photo is not null && photo.Length > 0)
        {
            dto.FilePath = await SavePhotoAsync(photo);
        }
        else
        {
            var existing = await _mediator.Send(new GetSitePhotoByIdQuery(dto.Id));
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

        var success = await _mediator.Send(new UpdateSitePhotoCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SitePhotoManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetSitePhotoActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> SavePhotoAsync(IFormFile photo)
    {
        var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "site-photos");
        Directory.CreateDirectory(uploadsRoot);
        var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(photo.FileName)}";
        var fullPath = Path.Combine(uploadsRoot, safeFileName);
        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await photo.CopyToAsync(stream);
        }

        return $"/uploads/site-photos/{safeFileName}";
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery());
        ViewBag.Projects = new SelectList(projects, "Id", "Name");

        var dailyProgresses = await _mediator.Send(new GetAllDailyProgressesQuery());
        ViewBag.DailyProgresses = new SelectList(dailyProgresses, "Id", "ProgressDate");
    }
}
