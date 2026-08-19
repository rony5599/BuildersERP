using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Documents;
using BuilderERP.Application.Features.DocumentVersions;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.DocumentVersionView)]
public class DocumentVersionsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateDocumentVersionDto> _createValidator;
    private readonly IValidator<UpdateDocumentVersionDto> _updateValidator;
    private readonly IWebHostEnvironment _environment;

    public DocumentVersionsController(
        IMediator mediator,
        IValidator<CreateDocumentVersionDto> createValidator,
        IValidator<UpdateDocumentVersionDto> updateValidator,
        IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _environment = environment;
    }

    public async Task<IActionResult> Index(Guid? documentId, int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllDocumentVersionsQuery(documentId, page, pageSize));
        ViewBag.SelectedDocumentId = documentId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.DocumentVersionManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateDocumentVersionDto { UploadedDate = DateTime.UtcNow });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DocumentVersionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDocumentVersionDto dto, IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            ModelState.AddModelError(nameof(file), "Please select a file to upload.");
        }
        else
        {
            var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "document-versions");
            Directory.CreateDirectory(uploadsRoot);
            var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, safeFileName);
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            dto.FilePath = $"/uploads/document-versions/{safeFileName}";
        }

        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!ModelState.IsValid || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateDocumentVersionCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.DocumentVersionManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetDocumentVersionByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateDocumentVersionDto
        {
            Id = item.Id,
            VersionNumber = item.VersionNumber,
            FilePath = item.FilePath,
            UploadedDate = item.UploadedDate,
            ChangeNotes = item.ChangeNotes,
            IsCurrent = item.IsCurrent,
            DocumentId = item.DocumentId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DocumentVersionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateDocumentVersionDto dto, IFormFile? file)
    {
        if (file is not null && file.Length > 0)
        {
            var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "document-versions");
            Directory.CreateDirectory(uploadsRoot);
            var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, safeFileName);
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            dto.FilePath = $"/uploads/document-versions/{safeFileName}";
        }
        else
        {
            var existing = await _mediator.Send(new GetDocumentVersionByIdQuery(dto.Id));
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

        var success = await _mediator.Send(new UpdateDocumentVersionCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DocumentVersionManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetDocumentVersionActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var documents = await _mediator.Send(new GetAllDocumentsQuery(PageSize: int.MaxValue));
        ViewBag.Documents = new SelectList(documents.Items, "Id", "DocumentNumber");
    }
}
