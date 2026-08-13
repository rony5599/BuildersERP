using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class SecurityIncidentsReportSpec : IReportSpec
{
    public string Key => "security-incidents";
    public string Name => "Security Incidents Report";
    public string Category => "Facility Management";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("IncidentNumber", "Incident #"),
        new ReportColumn("IncidentType", "Type"),
        new ReportColumn("Location", "Location"),
        new ReportColumn("Project", "Project"),
        new ReportColumn("IncidentDateTime", "Date/Time"),
        new ReportColumn("Severity", "Severity"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<SecurityIncident> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<SecurityIncident>().Query().Include(x => x.Project).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.IncidentNumber.Contains(f.Search) || (x.Location != null && x.Location.Contains(f.Search)));
        if (f.DateFrom.HasValue) q = q.Where(x => x.IncidentDateTime >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.IncidentDateTime <= f.DateTo);
        if (f.ProjectId.HasValue) q = q.Where(x => x.ProjectId == f.ProjectId);
        return q.OrderByDescending(x => x.IncidentDateTime);
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

    private static ReportRow ToRow(SecurityIncident x) => new(new Dictionary<string, string?>
    {
        ["IncidentNumber"] = x.IncidentNumber,
        ["IncidentType"] = x.IncidentType.ToString(),
        ["Location"] = x.Location,
        ["Project"] = x.Project?.Name,
        ["IncidentDateTime"] = x.IncidentDateTime.ToString("yyyy-MM-dd HH:mm"),
        ["Severity"] = x.Severity.ToString(),
        ["Status"] = x.Status.ToString(),
    });
}
