using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Customers;
using BuilderERP.Application.Features.InstallmentPlans;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Application.Features.SaleAgreements;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.InstallmentPlanView)]
public class InstallmentPlansController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateInstallmentPlanDto> _createValidator;
    private readonly IValidator<UpdateInstallmentPlanDto> _updateValidator;

    public InstallmentPlansController(IMediator mediator, IValidator<CreateInstallmentPlanDto> createValidator, IValidator<UpdateInstallmentPlanDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(long? projectId, long? propertyUnitId, long? customerId, int page = 1, int pageSize = 25)
    {
        var plans = await _mediator.Send(new GetAllInstallmentPlansQuery(projectId, propertyUnitId, customerId, page, pageSize));
        ViewBag.SelectedProjectId = projectId;
        ViewBag.SelectedPropertyUnitId = propertyUnitId;
        ViewBag.SelectedCustomerId = customerId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", plans);
        }

        await PopulateFiltersAsync(projectId, propertyUnitId, customerId);
        return View(plans);
    }

    [PermissionAuthorize(PermissionNames.InstallmentPlanManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateInstallmentPlanDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.InstallmentPlanManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInstallmentPlanDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateInstallmentPlanCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.InstallmentPlanManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var plan = await _mediator.Send(new GetInstallmentPlanByIdQuery(id));
        if (plan is null)
        {
            return NotFound();
        }

        var dto = new UpdateInstallmentPlanDto
        {
            Id = plan.Id,
            TotalAmount = plan.TotalAmount,
            NumberOfInstallments = plan.NumberOfInstallments,
            StartDate = plan.StartDate,
            InterestRatePercent = plan.InterestRatePercent,
            Status = plan.Status,
            SaleAgreementId = plan.SaleAgreementId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.InstallmentPlanManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateInstallmentPlanDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateInstallmentPlanCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.InstallmentPlanManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetInstallmentPlanActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var agreements = await _mediator.Send(new GetAllSaleAgreementsQuery(PageSize: int.MaxValue));
        ViewBag.SaleAgreements = agreements.Items;
    }

    private async Task PopulateFiltersAsync(long? projectId, long? propertyUnitId, long? customerId)
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);

        var units = await _mediator.Send(new GetAllPropertyUnitsQuery(projectId, PageSize: int.MaxValue));
        ViewBag.PropertyUnits = new SelectList(units.Items, "Id", "UnitNumber", propertyUnitId);

        var customers = await _mediator.Send(new GetAllCustomersQuery(PageSize: int.MaxValue));
        ViewBag.Customers = new SelectList(customers.Items, "Id", "FullName", customerId);
    }
}
