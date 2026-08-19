using BuilderERP.Application.Features.Leads;
using BuilderERP.Domain.Enums;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SalesPipelineView)]
public class SalesPipelineController : Controller
{
    private readonly IMediator _mediator;

    public SalesPipelineController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var leads = await _mediator.Send(new GetAllLeadsQuery(PageSize: int.MaxValue));
        return View(leads.Items.Where(l => l.IsActive).ToList());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.LeadManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MoveStage(Guid id, LeadStatus status)
    {
        await _mediator.Send(new SetLeadStatusCommand(id, status));
        return RedirectToAction(nameof(Index));
    }
}
