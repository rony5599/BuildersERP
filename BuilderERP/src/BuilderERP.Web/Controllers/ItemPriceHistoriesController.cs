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

    public async Task<IActionResult> Index(Guid? materialId, Guid? supplierId, int page = 1, int pageSize = 25)
    {
        var history = await _mediator.Send(new GetAllItemPriceHistoriesQuery(materialId, supplierId, page, pageSize));
        ViewBag.SelectedMaterialId = materialId;
        ViewBag.SelectedSupplierId = supplierId;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", history);
        }

        var materials = await _mediator.Send(new GetAllMaterialsQuery(PageSize: int.MaxValue));
        ViewBag.Materials = new SelectList(materials.Items, "Id", "Name", materialId);

        var suppliers = await _mediator.Send(new GetAllSuppliersQuery(PageSize: int.MaxValue));
        ViewBag.Suppliers = new SelectList(suppliers.Items, "Id", "Name", supplierId);

        return View(history);
    }
}
