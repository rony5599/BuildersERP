using System.Globalization;
using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class ProjectWiseProcurementReportSpec : IReportSpec
{
    public string Key => "project-wise-procurement";
    public string Name => "Project Wise Requisition, PO, GRN & Inventory";
    public string Category => "Procurement";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("Project", "Project"),
        new ReportColumn("MaterialCode", "Material Code"),
        new ReportColumn("Material", "Material"),
        new ReportColumn("UnitOfMeasure", "UOM"),
        new ReportColumn("RequisitionedQty", "Requisitioned Qty"),
        new ReportColumn("OrderedQty", "PO Qty"),
        new ReportColumn("ReceivedQty", "Received Qty (GRN)"),
        new ReportColumn("StockOnHand", "Stock On Hand"),
        new ReportColumn("PendingToOrder", "Pending To Order"),
        new ReportColumn("PendingToReceive", "Pending To Receive"),
    };

    private record RowKey(long ProjectId, long MaterialId);

    private class RowData
    {
        public string ProjectName { get; set; } = string.Empty;
        public string MaterialCode { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public string UnitOfMeasure { get; set; } = string.Empty;
        public decimal RequisitionedQty { get; set; }
        public decimal OrderedQty { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal StockOnHand { get; set; }
    }

    private async Task<List<RowData>> BuildRowsAsync(IUnitOfWork uow, ReportFilter f, CancellationToken ct)
    {
        var rows = new Dictionary<RowKey, RowData>();

        RowData GetOrAdd(long projectId, long materialId, string projectName, Material material)
        {
            var key = new RowKey(projectId, materialId);
            if (!rows.TryGetValue(key, out var row))
            {
                row = new RowData
                {
                    ProjectName = projectName,
                    MaterialCode = material.MaterialCode,
                    MaterialName = material.Name,
                    UnitOfMeasure = material.UnitOfMeasure.ToString(),
                };
                rows[key] = row;
            }
            return row;
        }

        // Purchase Requisitions
        var reqQuery = uow.Repository<PurchaseRequisitionDetail>().Query()
            .Include(x => x.Material)
            .Include(x => x.PurchaseRequisition).ThenInclude(x => x.Project)
            .Where(x => x.PurchaseRequisition.IsActive)
            .AsQueryable();
        if (f.ProjectId.HasValue) reqQuery = reqQuery.Where(x => x.PurchaseRequisition.ProjectId == f.ProjectId);
        if (f.DateFrom.HasValue) reqQuery = reqQuery.Where(x => x.PurchaseRequisition.RequestDate >= f.DateFrom);
        if (f.DateTo.HasValue) reqQuery = reqQuery.Where(x => x.PurchaseRequisition.RequestDate <= f.DateTo);
        var reqDetails = await reqQuery.ToListAsync(ct);
        foreach (var d in reqDetails)
        {
            var row = GetOrAdd(d.PurchaseRequisition.ProjectId, d.MaterialId, d.PurchaseRequisition.Project.Name, d.Material);
            row.RequisitionedQty += d.Quantity;
        }

        // Purchase Orders (linked to project via VendorQuotation -> Rfq -> PurchaseRequisition)
        var poQuery = uow.Repository<PurchaseOrderDetail>().Query()
            .Include(x => x.Material)
            .Include(x => x.PurchaseOrder).ThenInclude(x => x.VendorQuotation).ThenInclude(x => x.Rfq).ThenInclude(x => x.PurchaseRequisition).ThenInclude(x => x.Project)
            .Where(x => x.PurchaseOrder.IsActive)
            .AsQueryable();
        if (f.ProjectId.HasValue) poQuery = poQuery.Where(x => x.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition.ProjectId == f.ProjectId);
        if (f.DateFrom.HasValue) poQuery = poQuery.Where(x => x.PurchaseOrder.OrderDate >= f.DateFrom);
        if (f.DateTo.HasValue) poQuery = poQuery.Where(x => x.PurchaseOrder.OrderDate <= f.DateTo);
        var poDetails = await poQuery.ToListAsync(ct);
        foreach (var d in poDetails)
        {
            var requisition = d.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition;
            var row = GetOrAdd(requisition.ProjectId, d.MaterialId, requisition.Project.Name, d.Material);
            row.OrderedQty += d.OrderedQuantity;
        }

        // Goods Receives (linked to project via Warehouse)
        var grnQuery = uow.Repository<GoodsReceiveDetail>().Query()
            .Include(x => x.Material)
            .Include(x => x.GoodsReceive).ThenInclude(x => x.Warehouse).ThenInclude(x => x.Project)
            .Where(x => x.GoodsReceive.IsActive && x.GoodsReceive.Status != Domain.Enums.GrnStatus.Cancelled)
            .AsQueryable();
        if (f.ProjectId.HasValue) grnQuery = grnQuery.Where(x => x.GoodsReceive.Warehouse.ProjectId == f.ProjectId);
        if (f.DateFrom.HasValue) grnQuery = grnQuery.Where(x => x.GoodsReceive.ReceivedDate >= f.DateFrom);
        if (f.DateTo.HasValue) grnQuery = grnQuery.Where(x => x.GoodsReceive.ReceivedDate <= f.DateTo);
        var grnDetails = await grnQuery.ToListAsync(ct);
        foreach (var d in grnDetails)
        {
            var warehouse = d.GoodsReceive.Warehouse;
            var row = GetOrAdd(warehouse.ProjectId, d.MaterialId, warehouse.Project.Name, d.Material);
            row.ReceivedQty += d.ReceivedQuantity;
        }

        // Current Stock (linked to project via Warehouse); not date-filtered — represents current on-hand balance
        var stockQuery = uow.Repository<Stock>().Query()
            .Include(x => x.Material)
            .Include(x => x.Warehouse).ThenInclude(x => x.Project)
            .Where(x => x.IsActive)
            .AsQueryable();
        if (f.ProjectId.HasValue) stockQuery = stockQuery.Where(x => x.Warehouse.ProjectId == f.ProjectId);
        var stockItems = await stockQuery.ToListAsync(ct);
        foreach (var s in stockItems)
        {
            var row = GetOrAdd(s.Warehouse.ProjectId, s.MaterialId, s.Warehouse.Project.Name, s.Material);
            row.StockOnHand += s.QuantityOnHand;
        }

        var result = rows.Values.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(f.Search))
        {
            var search = f.Search;
            result = result.Where(x =>
                x.ProjectName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                x.MaterialName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                x.MaterialCode.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        return result.OrderBy(x => x.ProjectName).ThenBy(x => x.MaterialName).ToList();
    }

    public async Task<PagedResult<ReportRow>> QueryAsync(IUnitOfWork unitOfWork, ReportFilter filter, int page, int pageSize, CancellationToken cancellationToken)
    {
        var all = await BuildRowsAsync(unitOfWork, filter, cancellationToken);
        var total = all.Count;
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return new PagedResult<ReportRow>(items.Select(ToRow).ToList(), total, page, pageSize);
    }

    public async Task<IReadOnlyList<ReportRow>> QueryAllAsync(IUnitOfWork unitOfWork, ReportFilter filter, CancellationToken cancellationToken)
    {
        var all = await BuildRowsAsync(unitOfWork, filter, cancellationToken);
        return all.Take(10_000).Select(ToRow).ToList();
    }

    private static ReportRow ToRow(RowData x)
    {
        var pendingToOrder = x.RequisitionedQty - x.OrderedQty;
        var pendingToReceive = x.OrderedQty - x.ReceivedQty;
        return new ReportRow(new Dictionary<string, string?>
        {
            ["Project"] = x.ProjectName,
            ["MaterialCode"] = x.MaterialCode,
            ["Material"] = x.MaterialName,
            ["UnitOfMeasure"] = x.UnitOfMeasure,
            ["RequisitionedQty"] = x.RequisitionedQty.ToString("N2", CultureInfo.InvariantCulture),
            ["OrderedQty"] = x.OrderedQty.ToString("N2", CultureInfo.InvariantCulture),
            ["ReceivedQty"] = x.ReceivedQty.ToString("N2", CultureInfo.InvariantCulture),
            ["StockOnHand"] = x.StockOnHand.ToString("N2", CultureInfo.InvariantCulture),
            ["PendingToOrder"] = (pendingToOrder > 0 ? pendingToOrder : 0).ToString("N2", CultureInfo.InvariantCulture),
            ["PendingToReceive"] = (pendingToReceive > 0 ? pendingToReceive : 0).ToString("N2", CultureInfo.InvariantCulture),
        });
    }
}
