using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.VisitorLogs;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.VisitorLogView)]
public class VisitorLogsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateVisitorLogDto> _createValidator;
    private readonly IValidator<UpdateVisitorLogDto> _updateValidator;

    public VisitorLogsController(IMediator mediator, IValidator<CreateVisitorLogDto> createValidator, IValidator<UpdateVisitorLogDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllVisitorLogsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.VisitorLogManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateVisitorLogDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.VisitorLogManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVisitorLogDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateVisitorLogCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.VisitorLogManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetVisitorLogByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateVisitorLogDto
        {
            Id = item.Id,
            VisitorName = item.VisitorName,
            Phone = item.Phone,
            PurposeOfVisit = item.PurposeOfVisit,
            HostName = item.HostName,
            CheckInTime = item.CheckInTime,
            CheckOutTime = item.CheckOutTime,
            IdProofNumber = item.IdProofNumber,
            VehicleNumber = item.VehicleNumber,
            Remarks = item.Remarks,
            ProjectId = item.ProjectId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.VisitorLogManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateVisitorLogDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateVisitorLogCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.VisitorLogManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetVisitorLogActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
