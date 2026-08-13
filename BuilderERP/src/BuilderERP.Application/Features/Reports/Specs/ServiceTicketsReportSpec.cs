using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class ServiceTicketsReportSpec : IReportSpec
{
    public string Key => "service-tickets";
    public string Name => "Service Tickets Report";
    public string Category => "After Handover";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("TicketNumber", "Ticket #"),
        new ReportColumn("PropertyUnit", "Unit"),
        new ReportColumn("Subject", "Subject"),
        new ReportColumn("Category", "Category"),
        new ReportColumn("Priority", "Priority"),
        new ReportColumn("Status", "Status"),
        new ReportColumn("RaisedDate", "Raised"),
        new ReportColumn("ClosedDate", "Closed"),
        new ReportColumn("AssignedTo", "Assigned To"),
    };

    private static IQueryable<ServiceTicket> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<ServiceTicket>().Query().Include(x => x.PropertyUnit).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.TicketNumber.Contains(f.Search) || x.Subject.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.RaisedDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.RaisedDate <= f.DateTo);
        return q.OrderByDescending(x => x.RaisedDate);
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

    private static ReportRow ToRow(ServiceTicket x) => new(new Dictionary<string, string?>
    {
        ["TicketNumber"] = x.TicketNumber,
        ["PropertyUnit"] = x.PropertyUnit?.UnitNumber,
        ["Subject"] = x.Subject,
        ["Category"] = x.Category.ToString(),
        ["Priority"] = x.Priority.ToString(),
        ["Status"] = x.Status.ToString(),
        ["RaisedDate"] = x.RaisedDate.ToString("yyyy-MM-dd"),
        ["ClosedDate"] = x.ClosedDate?.ToString("yyyy-MM-dd"),
        ["AssignedTo"] = x.AssignedTo,
    });
}
