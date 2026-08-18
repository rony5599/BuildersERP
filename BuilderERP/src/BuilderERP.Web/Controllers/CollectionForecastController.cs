using BuilderERP.Application.Features.CollectionForecast;
using BuilderERP.Application.Features.Customers;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Domain.Entities;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.DueCollectionForecastView)]
public class CollectionForecastController : Controller
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;

    public CollectionForecastController(IMediator mediator, UserManager<ApplicationUser> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(Guid? projectId, Guid? propertyUnitId, Guid? customerId, Guid? collectionOfficerId, string granularity = "Monthly")
    {
        var summary = await _mediator.Send(new GetDueCollectionForecastSummaryQuery(projectId, propertyUnitId, customerId, collectionOfficerId));
        var buckets = await _mediator.Send(new GetDueCollectionForecastBucketsQuery(granularity, projectId, propertyUnitId, customerId, collectionOfficerId));

        await PopulateFiltersAsync(projectId);
        ViewBag.SelectedProjectId = projectId;
        ViewBag.SelectedPropertyUnitId = propertyUnitId;
        ViewBag.SelectedCustomerId = customerId;
        ViewBag.SelectedCollectionOfficerId = collectionOfficerId;
        ViewBag.Granularity = granularity;
        ViewBag.Buckets = buckets;

        return View(summary);
    }

    public async Task<IActionResult> HighRiskDefaulters(Guid? projectId)
    {
        var defaulters = await _mediator.Send(new GetHighRiskDefaultersQuery(projectId));

        var projects = await _mediator.Send(new GetAllProjectsQuery());
        ViewBag.Projects = new SelectList(projects, "Id", "Name", projectId);
        ViewBag.SelectedProjectId = projectId;

        return View(defaulters);
    }

    private async Task PopulateFiltersAsync(Guid? projectId)
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery());
        ViewBag.Projects = new SelectList(projects, "Id", "Name");

        var units = await _mediator.Send(new GetAllPropertyUnitsQuery(projectId));
        ViewBag.PropertyUnits = new SelectList(units, "Id", "UnitNumber");

        var customers = await _mediator.Send(new GetAllCustomersQuery());
        ViewBag.Customers = new SelectList(customers, "Id", "FullName");

        var users = await _userManager.Users.ToListAsync();
        ViewBag.CollectionOfficers = new SelectList(users, "Id", "FullName");
    }
}
