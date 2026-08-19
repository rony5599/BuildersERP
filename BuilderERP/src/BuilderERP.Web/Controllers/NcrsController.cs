using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Ncrs;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.NcrView)]
public class NcrsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateNcrDto> _createValidator;
    private readonly IValidator<UpdateNcrDto> _updateValidator;

    public NcrsController(IMediator mediator, IValidator<CreateNcrDto> createValidator, IValidator<UpdateNcrDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(Guid? projectId, int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllNcrsQuery(projectId, page, pageSize));
        ViewBag.SelectedProjectId = projectId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.NcrManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateNcrDto { RaisedDate = DateTime.UtcNow });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.NcrManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateNcrDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateNcrCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.NcrManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetNcrByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateNcrDto
        {
            Id = item.Id,
            NcrNumber = item.NcrNumber,
            RaisedDate = item.RaisedDate,
            Description = item.Description,
            Severity = item.Severity,
            Status = item.Status,
            ResolutionDescription = item.ResolutionDescription,
            ClosedDate = item.ClosedDate,
            ProjectId = item.ProjectId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.NcrManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateNcrDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateNcrCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.NcrManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetNcrActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
