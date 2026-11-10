using BuilderERP.Application.Features.Rfqs;
using BuilderERP.Application.Features.VendorQuotations;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.VendorQuotationView)]
public class VendorCompareController : Controller
{
    private readonly IMediator _mediator;

    public VendorCompareController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(long? rfqId = null)
    {
        var rfqs = await _mediator.Send(new GetAllRfqsQuery(PageSize: int.MaxValue));
        var selectedId = rfqId ?? rfqs.Items.FirstOrDefault()?.Id;
        var selectedRfq = selectedId.HasValue
            ? await _mediator.Send(new GetRfqByIdQuery(selectedId.Value))
            : null;

        if (rfqId.HasValue && selectedRfq is null)
        {
            return NotFound();
        }

        var quotations = selectedId.HasValue
            ? await _mediator.Send(new GetVendorQuotationsByRfqIdQuery(selectedId.Value))
            : [];

        return View(new VendorCompareViewModel
        {
            SelectedRfqId = selectedId,
            Rfq = selectedRfq,
            Rfqs = rfqs.Items,
            Quotations = quotations
        });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.VendorQuotationManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SelectWinner(long id, long rfqId)
    {
        var selected = await _mediator.Send(new SelectWinnerVendorQuotationCommand(id));
        if (!selected)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index), new { rfqId });
    }
}
