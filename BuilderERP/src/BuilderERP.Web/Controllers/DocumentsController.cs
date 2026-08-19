using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Customers;
using BuilderERP.Application.Features.Documents;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.DocumentView)]
public class DocumentsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateDocumentDto> _createValidator;
    private readonly IValidator<UpdateDocumentDto> _updateValidator;
    private readonly IWebHostEnvironment _environment;

    public DocumentsController(
        IMediator mediator,
        IValidator<CreateDocumentDto> createValidator,
        IValidator<UpdateDocumentDto> updateValidator,
        IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _environment = environment;
    }

    public async Task<IActionResult> Index(Guid? customerId, Guid? projectId, int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllDocumentsQuery(customerId, projectId, page, pageSize));
        ViewBag.SelectedCustomerId = customerId;
        ViewBag.SelectedProjectId = projectId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.DocumentManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateDocumentDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DocumentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDocumentDto dto, IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            ModelState.AddModelError(nameof(file), "Please select a file to upload.");
        }
        else
        {
            var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "documents");
            Directory.CreateDirectory(uploadsRoot);
            var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, safeFileName);
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            dto.FilePath = $"/uploads/documents/{safeFileName}";
        }

        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!ModelState.IsValid || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateDocumentCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.DocumentManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetDocumentByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateDocumentDto
        {
            Id = item.Id,
            DocumentNumber = item.DocumentNumber,
            Title = item.Title,
            DocumentType = item.DocumentType,
            FilePath = item.FilePath,
            IssueDate = item.IssueDate,
            ExpiryDate = item.ExpiryDate,
            Remarks = item.Remarks,
            CustomerId = item.CustomerId,
            ProjectId = item.ProjectId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DocumentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateDocumentDto dto, IFormFile? file)
    {
        if (file is not null && file.Length > 0)
        {
            var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "documents");
            Directory.CreateDirectory(uploadsRoot);
            var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, safeFileName);
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            dto.FilePath = $"/uploads/documents/{safeFileName}";
        }
        else
        {
            var existing = await _mediator.Send(new GetDocumentByIdQuery(dto.Id));
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

        var success = await _mediator.Send(new UpdateDocumentCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DocumentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetDocumentActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var customers = await _mediator.Send(new GetAllCustomersQuery(PageSize: int.MaxValue));
        ViewBag.Customers = new SelectList(customers.Items, "Id", "FullName");

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
