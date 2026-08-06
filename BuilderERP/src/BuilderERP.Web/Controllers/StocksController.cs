using BuilderERP.Application.Features.Stocks;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.StockView)]
public class StocksController : Controller
{
    private readonly IMediator _mediator;

    public StocksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var stock = await _mediator.Send(new GetAllStockQuery());
        return View(stock);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.StockManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetStockActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }
}
