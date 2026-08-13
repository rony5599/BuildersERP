using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class MaintenanceRequestsReportSpec : IReportSpec
{
    public string Key => "maintenance-requests";
    public string Name => "Maintenance Requests Report";
    public string Category => "After Handover";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("RequestNumber", "Request #"),
        new ReportColumn("PropertyUnit", "Unit"),
        new ReportColumn("RequestType", "Type"),
        new ReportColumn("Priority", "Priority"),
        new ReportColumn("Status", "Status"),
        new ReportColumn("RequestDate", "Requested"),
        new ReportColumn("ResolvedDate", "Resolved"),
        new ReportColumn("AssignedTo", "Assigned To"),
    };

    private static IQueryable<MaintenanceRequest> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<MaintenanceRequest>().Query().Include(x => x.PropertyUnit).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.RequestNumber.Contains(f.Search) || x.Description.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.RequestDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.RequestDate <= f.DateTo);
        return q.OrderByDescending(x => x.RequestDate);
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

    private static ReportRow ToRow(MaintenanceRequest x) => new(new Dictionary<string, string?>
    {
        ["RequestNumber"] = x.RequestNumber,
        ["PropertyUnit"] = x.PropertyUnit?.UnitNumber,
        ["RequestType"] = x.RequestType.ToString(),
        ["Priority"] = x.Priority.ToString(),
        ["Status"] = x.Status.ToString(),
        ["RequestDate"] = x.RequestDate.ToString("yyyy-MM-dd"),
        ["ResolvedDate"] = x.ResolvedDate?.ToString("yyyy-MM-dd"),
        ["AssignedTo"] = x.AssignedTo,
    });
}
