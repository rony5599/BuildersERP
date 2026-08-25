using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.LandDocuments;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.LandDocumentView)]
public class LandDocumentsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateLandDocumentDto> _createValidator;
    private readonly IValidator<UpdateLandDocumentDto> _updateValidator;

    public LandDocumentsController(IMediator mediator, IValidator<CreateLandDocumentDto> createValidator, IValidator<UpdateLandDocumentDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllLandDocumentsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.LandDocumentManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateLandDocumentDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LandDocumentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLandDocumentDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateLandDocumentCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.LandDocumentManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var item = await _mediator.Send(new GetLandDocumentByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateLandDocumentDto
        {
            Id = item.Id,
            DocumentNumber = item.DocumentNumber,
            Title = item.Title,
            LandDocumentType = item.LandDocumentType,
            MouzaName = item.MouzaName,
            JlNumber = item.JlNumber,
            KhatianNumber = item.KhatianNumber,
            DagNumber = item.DagNumber,
            AreaInDecimal = item.AreaInDecimal,
            AcquisitionDate = item.AcquisitionDate,
            Remarks = item.Remarks,
            ProjectId = item.ProjectId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LandDocumentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateLandDocumentDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateLandDocumentCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LandDocumentManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetLandDocumentActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
