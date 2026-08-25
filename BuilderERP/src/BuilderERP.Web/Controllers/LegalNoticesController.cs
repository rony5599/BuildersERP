using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.LegalNotices;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.LegalNoticeView)]
public class LegalNoticesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateLegalNoticeDto> _createValidator;
    private readonly IValidator<UpdateLegalNoticeDto> _updateValidator;

    public LegalNoticesController(IMediator mediator, IValidator<CreateLegalNoticeDto> createValidator, IValidator<UpdateLegalNoticeDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllLegalNoticesQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.LegalNoticeManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateLegalNoticeDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LegalNoticeManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLegalNoticeDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateLegalNoticeCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.LegalNoticeManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var item = await _mediator.Send(new GetLegalNoticeByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateLegalNoticeDto
        {
            Id = item.Id,
            NoticeNumber = item.NoticeNumber,
            Title = item.Title,
            NoticeType = item.NoticeType,
            IssuedTo = item.IssuedTo,
            IssueDate = item.IssueDate,
            ResponseDeadline = item.ResponseDeadline,
            Status = item.Status,
            Remarks = item.Remarks,
            ProjectId = item.ProjectId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LegalNoticeManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateLegalNoticeDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateLegalNoticeCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LegalNoticeManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetLegalNoticeActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
