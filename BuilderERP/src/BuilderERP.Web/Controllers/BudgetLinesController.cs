using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.BudgetLines;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.BudgetLineView)]
public class BudgetLinesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateBudgetLineDto> _createValidator;
    private readonly IValidator<UpdateBudgetLineDto> _updateValidator;

    public BudgetLinesController(IMediator mediator, IValidator<CreateBudgetLineDto> createValidator, IValidator<UpdateBudgetLineDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(Guid? projectId, int page = 1, int pageSize = 25)
    {
        var budgetLines = await _mediator.Send(new GetAllBudgetLinesQuery(projectId, page, pageSize));
        ViewBag.SelectedProjectId = projectId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", budgetLines);
        }

        return View(budgetLines);
    }

    [PermissionAuthorize(PermissionNames.BudgetLineView)]
    public async Task<IActionResult> Report(Guid? projectId)
    {
        var projects = await _mediator.Send(new BuilderERP.Application.Features.Projects.GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);

        var lines = await _mediator.Send(new GetAllBudgetLinesQuery(projectId, PageSize: int.MaxValue));
        return View(lines.Items);
    }

    [PermissionAuthorize(PermissionNames.BudgetLineManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateBudgetLineDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BudgetLineManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBudgetLineDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateBudgetLineCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.BudgetLineManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var budgetLine = await _mediator.Send(new GetBudgetLineByIdQuery(id));
        if (budgetLine is null)
        {
            return NotFound();
        }

        var dto = new UpdateBudgetLineDto
        {
            Id = budgetLine.Id,
            Category = budgetLine.Category,
            PeriodStart = budgetLine.PeriodStart,
            BudgetedAmount = budgetLine.BudgetedAmount,
            ActualAmount = budgetLine.ActualAmount,
            Remarks = budgetLine.Remarks,
            ProjectId = budgetLine.ProjectId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BudgetLineManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateBudgetLineDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateBudgetLineCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BudgetLineManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetBudgetLineActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var projects = await _mediator.Send(new BuilderERP.Application.Features.Projects.GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name");
    }
}
