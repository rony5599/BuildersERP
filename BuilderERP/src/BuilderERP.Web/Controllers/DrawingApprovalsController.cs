using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.DrawingApprovals;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.DrawingApprovalView)]
public class DrawingApprovalsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateDrawingApprovalDto> _createValidator;
    private readonly IValidator<UpdateDrawingApprovalDto> _updateValidator;

    public DrawingApprovalsController(IMediator mediator, IValidator<CreateDrawingApprovalDto> createValidator, IValidator<UpdateDrawingApprovalDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(long? drawingId, int page = 1, int pageSize = 25)
    {
        var approvals = await _mediator.Send(new GetAllDrawingApprovalsQuery(drawingId, page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", approvals);
        }

        return View(approvals);
    }

    [PermissionAuthorize(PermissionNames.DrawingApprovalManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateDrawingApprovalDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DrawingApprovalManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDrawingApprovalDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateDrawingApprovalCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.DrawingApprovalManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var approval = await _mediator.Send(new GetDrawingApprovalByIdQuery(id));
        if (approval is null)
        {
            return NotFound();
        }

        var dto = new UpdateDrawingApprovalDto
        {
            Id = approval.Id,
            DrawingId = approval.DrawingId,
            DrawingRevisionId = approval.DrawingRevisionId,
            ApproverName = approval.ApproverName,
            Status = approval.Status,
            Comments = approval.Comments,
            RequestedDate = approval.RequestedDate,
            ActionDate = approval.ActionDate
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DrawingApprovalManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateDrawingApprovalDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateDrawingApprovalCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DrawingApprovalManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetDrawingApprovalActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var drawings = await _mediator.Send(new BuilderERP.Application.Features.Drawings.GetAllDrawingsQuery(PageSize: int.MaxValue));
        ViewBag.Drawings = new SelectList(drawings.Items, "Id", "DrawingNumber");

        var revisions = await _mediator.Send(new BuilderERP.Application.Features.DrawingRevisions.GetAllDrawingRevisionsQuery(PageSize: int.MaxValue));
        ViewBag.DrawingRevisions = new SelectList(revisions.Items, "Id", "RevisionCode");
    }
}
