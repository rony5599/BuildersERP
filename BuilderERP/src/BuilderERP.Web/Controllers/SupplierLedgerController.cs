using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Suppliers;
using BuilderERP.Application.Features.SupplierPayments;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SupplierLedgerView)]
public class SupplierLedgerController : Controller
{
    private readonly IMediator _mediator;

    public SupplierLedgerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(long? supplierId = null, DateTime? dateFrom = null, DateTime? dateTo = null, SupplierLedgerSource source = SupplierLedgerSource.All)
    {
        var suppliers = await _mediator.Send(new GetAllSuppliersQuery(PageSize: int.MaxValue));
        ViewBag.Suppliers = new SelectList(suppliers.Items.OrderBy(s => s.Name), "Id", "Name", supplierId);
        ViewBag.SupplierId = supplierId;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;
        ViewBag.Source = source;

        if (!supplierId.HasValue)
        {
            return View(null as BuilderERP.Application.DTOs.SupplierLedgerDto);
        }

        var ledger = await _mediator.Send(new GetSupplierLedgerQuery(supplierId.Value, dateFrom, dateTo, source));
        if (ledger is null)
        {
            return NotFound();
        }

        return View(ledger);
    }

    public async Task<IActionResult> Print(long supplierId, DateTime? dateFrom = null, DateTime? dateTo = null, SupplierLedgerSource source = SupplierLedgerSource.All)
    {
        var ledger = await _mediator.Send(new GetSupplierLedgerQuery(supplierId, dateFrom, dateTo, source));
        if (ledger is null)
        {
            return NotFound();
        }

        ledger.PrintedBy = User.Identity?.Name;
        ledger.PrintedAt = DateTime.Now;
        return View(ledger);
    }
}
