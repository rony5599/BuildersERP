using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class FlatHandoversReportSpec : IReportSpec
{
    public string Key => "flat-handovers";
    public string Name => "Flat Handovers Report";
    public string Category => "After Handover";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("HandoverNumber", "Handover #"),
        new ReportColumn("PropertyUnit", "Unit"),
        new ReportColumn("Customer", "Customer"),
        new ReportColumn("HandoverDate", "Handover Date"),
        new ReportColumn("KeyIssuedTo", "Key Issued To"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<FlatHandover> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<FlatHandover>().Query().Include(x => x.PropertyUnit).Include(x => x.Customer).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.HandoverNumber.Contains(f.Search) || x.Customer.FullName.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.HandoverDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.HandoverDate <= f.DateTo);
        return q.OrderByDescending(x => x.HandoverDate);
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

    private static ReportRow ToRow(FlatHandover x) => new(new Dictionary<string, string?>
    {
        ["HandoverNumber"] = x.HandoverNumber,
        ["PropertyUnit"] = x.PropertyUnit?.UnitNumber,
        ["Customer"] = x.Customer?.FullName,
        ["HandoverDate"] = x.HandoverDate.ToString("yyyy-MM-dd"),
        ["KeyIssuedTo"] = x.KeyIssuedTo,
        ["Status"] = x.Status.ToString(),
    });
}
