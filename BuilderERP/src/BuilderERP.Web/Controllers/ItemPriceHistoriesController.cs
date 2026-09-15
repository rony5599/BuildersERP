using BuilderERP.Application.Features.ItemPriceHistories;
using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Suppliers;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.ItemPriceHistoryView)]
public class ItemPriceHistoriesController : Controller
{
    private readonly IMediator _mediator;

    public ItemPriceHistoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(long? materialId, long? supplierId, int page = 1, int pageSize = 25, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var history = await _mediator.Send(new GetAllItemPriceHistoriesQuery(materialId, supplierId, page, pageSize, dateFrom, dateTo));
        ViewBag.SelectedMaterialId = materialId;
        ViewBag.SelectedSupplierId = supplierId;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

        var materials = await _mediator.Send(new GetAllMaterialsQuery(PageSize: int.MaxValue));
        ViewBag.Materials = new SelectList(materials.Items, "Id", "Name", materialId);

        var suppliers = await _mediator.Send(new GetAllSuppliersQuery(PageSize: int.MaxValue));
        ViewBag.Suppliers = new SelectList(suppliers.Items, "Id", "Name", supplierId);

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", history);
        }

        return View(history);
    }
}
