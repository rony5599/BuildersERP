using BuilderERP.Application.Features.Materials;
using BuilderERP.Application.Features.Projects;
using BuilderERP.Application.Features.Stocks;
using BuilderERP.Application.Features.Warehouses;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.StockView)]
public class StocksController : Controller
{
    private readonly IMediator _mediator;

    public StocksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(long? projectId, int page = 1, int pageSize = 25, long? materialId = null, long? warehouseId = null, bool? isActive = null)
    {
        var stock = await _mediator.Send(new GetAllStockQuery(projectId, page, pageSize, materialId, warehouseId, isActive));
        ViewBag.SelectedProjectId = projectId;
        ViewBag.SelectedMaterialId = materialId;
        ViewBag.SelectedWarehouseId = warehouseId;
        ViewBag.IsActive = isActive;

        var materials = await _mediator.Send(new GetAllMaterialsQuery(PageSize: int.MaxValue));
        ViewBag.Materials = new SelectList(materials.Items, "Id", "Name", materialId);

        var warehouses = await _mediator.Send(new GetAllWarehousesQuery(PageSize: int.MaxValue));
        ViewBag.Warehouses = new SelectList(warehouses.Items, "Id", "Name", warehouseId);

        var projects = await _mediator.Send(new GetAllProjectsQuery(PageSize: int.MaxValue));
        ViewBag.Projects = new SelectList(projects.Items, "Id", "Name", projectId);

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", stock);
        }

        return View(stock);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.StockManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetStockActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }
}
