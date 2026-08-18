using BuilderERP.Application.Features.AuditTrail;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.AuditTrailView)]
public class AuditTrailController : Controller
{
    private readonly IMediator _mediator;

    public AuditTrailController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index([FromQuery] AuditLogFilter filter, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAuditLogsQuery(filter, page, pageSize), cancellationToken);
        var entityNames = await _mediator.Send(new GetAuditEntityNamesQuery(), cancellationToken);

        ViewBag.EntityNames = new SelectList(entityNames, filter.EntityName);
        ViewBag.Filter = filter;
        return View(result);
    }

    // Lets any module's Details/Edit page deep-link to that record's change history,
    // e.g. /AuditTrail/History?entityName=Material&entityId={id}
    public async Task<IActionResult> History(string entityName, Guid entityId, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var filter = new AuditLogFilter { EntityName = entityName, EntityId = entityId };
        var result = await _mediator.Send(new GetAuditLogsQuery(filter, page, pageSize), cancellationToken);

        ViewBag.EntityNames = new SelectList(new[] { entityName }, entityName);
        ViewBag.Filter = filter;
        ViewBag.IsHistoryView = true;
        return View("Index", result);
    }
}
