using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.RequesterLedger;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.RequesterLedgerView)]
public class RequesterLedgerController : Controller
{
    private readonly IMediator _mediator;

    public RequesterLedgerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(long? employeeId = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var requesters = await _mediator.Send(new GetRequesterOptionsQuery());
        ViewBag.Requesters = new SelectList(requesters, "Id", "Name", employeeId);
        ViewBag.EmployeeId = employeeId;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

        if (!employeeId.HasValue)
        {
            return View(null as RequesterLedgerDto);
        }

        var ledger = await _mediator.Send(new GetRequesterLedgerQuery(employeeId.Value, dateFrom, dateTo));
        if (ledger is null)
        {
            return NotFound();
        }

        return View(ledger);
    }

    public async Task<IActionResult> Print(long employeeId, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var ledger = await _mediator.Send(new GetRequesterLedgerQuery(employeeId, dateFrom, dateTo));
        if (ledger is null)
        {
            return NotFound();
        }

        ledger.PrintedBy = User.Identity?.Name;
        ledger.PrintedAt = DateTime.Now;
        return View(ledger);
    }
}
