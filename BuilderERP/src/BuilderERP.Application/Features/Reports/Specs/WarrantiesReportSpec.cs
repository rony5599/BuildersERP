using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class WarrantiesReportSpec : IReportSpec
{
    public string Key => "warranties";
    public string Name => "Warranties Report";
    public string Category => "After Handover";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("WarrantyNumber", "Warranty #"),
        new ReportColumn("PropertyUnit", "Unit"),
        new ReportColumn("ItemCovered", "Item Covered"),
        new ReportColumn("WarrantyType", "Type"),
        new ReportColumn("StartDate", "Start"),
        new ReportColumn("EndDate", "End"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<Warranty> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<Warranty>().Query().Include(x => x.PropertyUnit).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.WarrantyNumber.Contains(f.Search) || x.ItemCovered.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.StartDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.StartDate <= f.DateTo);
        return q.OrderByDescending(x => x.StartDate);
    }

    public async Task<PagedResult<ReportRow>> QueryAsync(IUnitOfWork unitOfWork, ReportFilter filter, int page, int pageSize, CancellationToken cancellationToken)
    {
        var q = BaseQuery(unitOfWork, filter);
        var total = await q.CountAsync(cancellationToken);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<ReportRow>(items.Select(ToRow).ToList(), total, page, pageSize);
    }

    public async Task<IReadOnlyList<ReportRow>> QueryAllAsync(IUnitOfWork unitOfWork, ReportFilter filter, CancellationToken cancellationToken)
    {
        var items = await BaseQuery(unitOfWork, filter).Take(10_000).ToListAsync(cancellationToken);
        return items.Select(ToRow).ToList();
    }

    private static ReportRow ToRow(Warranty x) => new(new Dictionary<string, string?>
    {
        ["WarrantyNumber"] = x.WarrantyNumber,
        ["PropertyUnit"] = x.PropertyUnit?.UnitNumber,
        ["ItemCovered"] = x.ItemCovered,
        ["WarrantyType"] = x.WarrantyType.ToString(),
        ["StartDate"] = x.StartDate.ToString("yyyy-MM-dd"),
        ["EndDate"] = x.EndDate.ToString("yyyy-MM-dd"),
        ["Status"] = x.Status.ToString(),
    });
}
