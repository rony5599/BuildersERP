using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class SnagItemsReportSpec : IReportSpec
{
    public string Key => "snag-items";
    public string Name => "Snag Items Report";
    public string Category => "After Handover";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("SnagNumber", "Snag #"),
        new ReportColumn("PropertyUnit", "Unit"),
        new ReportColumn("Description", "Description"),
        new ReportColumn("Severity", "Severity"),
        new ReportColumn("Status", "Status"),
        new ReportColumn("ReportedDate", "Reported"),
        new ReportColumn("ResolvedDate", "Resolved"),
    };

    private static IQueryable<SnagItem> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<SnagItem>().Query().Include(x => x.PropertyUnit).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.SnagNumber.Contains(f.Search) || x.Description.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.ReportedDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.ReportedDate <= f.DateTo);
        return q.OrderByDescending(x => x.ReportedDate);
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

    private static ReportRow ToRow(SnagItem x) => new(new Dictionary<string, string?>
    {
        ["SnagNumber"] = x.SnagNumber,
        ["PropertyUnit"] = x.PropertyUnit?.UnitNumber,
        ["Description"] = x.Description,
        ["Severity"] = x.Severity.ToString(),
        ["Status"] = x.Status.ToString(),
        ["ReportedDate"] = x.ReportedDate.ToString("yyyy-MM-dd"),
        ["ResolvedDate"] = x.ResolvedDate?.ToString("yyyy-MM-dd"),
    });
}
