using System.Globalization;
using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class BookingsReportSpec : IReportSpec
{
    public string Key => "bookings";
    public string Name => "Bookings Report";
    public string Category => "Sales";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("Customer", "Customer"),
        new ReportColumn("PropertyUnit", "Unit"),
        new ReportColumn("BookingDate", "Booking Date"),
        new ReportColumn("BookingAmount", "Amount"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<Booking> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<Booking>().Query().Include(x => x.Customer).Include(x => x.PropertyUnit).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.Customer.FullName.Contains(f.Search) || x.PropertyUnit.UnitNumber.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.BookingDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.BookingDate <= f.DateTo);
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

    private static ReportRow ToRow(Booking x) => new(new Dictionary<string, string?>
    {
        ["Customer"] = x.Customer?.FullName,
        ["PropertyUnit"] = x.PropertyUnit?.UnitNumber,
        ["BookingDate"] = x.BookingDate.ToString("yyyy-MM-dd"),
        ["BookingAmount"] = x.BookingAmount.ToString("N2", CultureInfo.InvariantCulture),
        ["Status"] = x.Status.ToString(),
    });
}
