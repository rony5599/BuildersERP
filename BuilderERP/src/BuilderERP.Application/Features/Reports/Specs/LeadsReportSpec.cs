using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class LeadsReportSpec : IReportSpec
{
    public string Key => "leads";
    public string Name => "Leads Report";
    public string Category => "CRM";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("Name", "Name"),
        new ReportColumn("Phone", "Phone"),
        new ReportColumn("Email", "Email"),
        new ReportColumn("Source", "Source"),
        new ReportColumn("AssignedTo", "Assigned To"),
        new ReportColumn("Status", "Status"),
        new ReportColumn("CreatedAt", "Created"),
    };

    private static IQueryable<Lead> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<Lead>().Query().Include(x => x.AssignedToUser).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.Name.Contains(f.Search) || x.Phone.Contains(f.Search) || (x.Email != null && x.Email.Contains(f.Search)));
        if (f.DateFrom.HasValue) q = q.Where(x => x.CreatedAt >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.CreatedAt <= f.DateTo);
        return q.OrderByDescending(x => x.CreatedAt);
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

    private static ReportRow ToRow(Lead x) => new(new Dictionary<string, string?>
    {
        ["Name"] = x.Name,
        ["Phone"] = x.Phone,
        ["Email"] = x.Email,
        ["Source"] = x.Source,
        ["AssignedTo"] = x.AssignedToUser?.UserName,
        ["Status"] = x.Status.ToString(),
        ["CreatedAt"] = x.CreatedAt.ToString("yyyy-MM-dd"),
    });
}
