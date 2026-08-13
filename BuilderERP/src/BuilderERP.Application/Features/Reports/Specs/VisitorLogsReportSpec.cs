using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class VisitorLogsReportSpec : IReportSpec
{
    public string Key => "visitor-logs";
    public string Name => "Visitor Logs Report";
    public string Category => "Facility Management";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("VisitorName", "Visitor"),
        new ReportColumn("Phone", "Phone"),
        new ReportColumn("PurposeOfVisit", "Purpose"),
        new ReportColumn("HostName", "Host"),
        new ReportColumn("Project", "Project"),
        new ReportColumn("CheckInTime", "Check In"),
        new ReportColumn("CheckOutTime", "Check Out"),
    };

    private static IQueryable<VisitorLog> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<VisitorLog>().Query().Include(x => x.Project).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.VisitorName.Contains(f.Search) || (x.Phone != null && x.Phone.Contains(f.Search)));
        if (f.DateFrom.HasValue) q = q.Where(x => x.CheckInTime >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.CheckInTime <= f.DateTo);
        if (f.ProjectId.HasValue) q = q.Where(x => x.ProjectId == f.ProjectId);
        return q.OrderByDescending(x => x.CheckInTime);
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

    private static ReportRow ToRow(VisitorLog x) => new(new Dictionary<string, string?>
    {
        ["VisitorName"] = x.VisitorName,
        ["Phone"] = x.Phone,
        ["PurposeOfVisit"] = x.PurposeOfVisit,
        ["HostName"] = x.HostName,
        ["Project"] = x.Project?.Name,
        ["CheckInTime"] = x.CheckInTime.ToString("yyyy-MM-dd HH:mm"),
        ["CheckOutTime"] = x.CheckOutTime?.ToString("yyyy-MM-dd HH:mm"),
    });
}
