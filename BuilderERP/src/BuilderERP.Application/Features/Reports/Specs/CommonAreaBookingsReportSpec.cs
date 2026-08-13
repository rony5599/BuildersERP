using System.Globalization;
using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class CommonAreaBookingsReportSpec : IReportSpec
{
    public string Key => "common-area-bookings";
    public string Name => "Common Area Bookings Report";
    public string Category => "Facility Management";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("BookingNumber", "Booking #"),
        new ReportColumn("FacilityName", "Facility"),
        new ReportColumn("Project", "Project"),
        new ReportColumn("BookingDate", "Booking Date"),
        new ReportColumn("StartTime", "Start"),
        new ReportColumn("EndTime", "End"),
        new ReportColumn("Fee", "Fee"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<CommonAreaBooking> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<CommonAreaBooking>().Query().Include(x => x.Project).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.BookingNumber.Contains(f.Search) || x.FacilityName.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.BookingDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.BookingDate <= f.DateTo);
        if (f.ProjectId.HasValue) q = q.Where(x => x.ProjectId == f.ProjectId);
        return q.OrderByDescending(x => x.BookingDate);
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

    private static ReportRow ToRow(CommonAreaBooking x) => new(new Dictionary<string, string?>
    {
        ["BookingNumber"] = x.BookingNumber,
        ["FacilityName"] = x.FacilityName,
        ["Project"] = x.Project?.Name,
        ["BookingDate"] = x.BookingDate.ToString("yyyy-MM-dd"),
        ["StartTime"] = x.StartTime.ToString("HH:mm"),
        ["EndTime"] = x.EndTime.ToString("HH:mm"),
        ["Fee"] = x.Fee.ToString("N2", CultureInfo.InvariantCulture),
        ["Status"] = x.Status.ToString(),
    });
}
