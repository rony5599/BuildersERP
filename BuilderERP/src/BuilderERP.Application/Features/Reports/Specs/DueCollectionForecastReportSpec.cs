using System.Globalization;
using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class DueCollectionForecastReportSpec : IReportSpec
{
    public string Key => "due-collection-forecast";
    public string Name => "Due Collection Forecast";
    public string Category => "Installments";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("CustomerName", "Customer"),
        new ReportColumn("Phone", "Phone"),
        new ReportColumn("ProjectName", "Project"),
        new ReportColumn("UnitNumber", "Unit"),
        new ReportColumn("DueDate", "Due Date"),
        new ReportColumn("OutstandingAmount", "Outstanding"),
        new ReportColumn("DaysOverdue", "Days Overdue"),
        new ReportColumn("Bucket", "Bucket"),
        new ReportColumn("Rescheduled", "Rescheduled"),
        new ReportColumn("CollectionOfficer", "Collection Officer"),
    };

    private static async Task<List<Installment>> BaseQuery(IUnitOfWork uow, ReportFilter f, CancellationToken cancellationToken)
    {
        var q = uow.Repository<Installment>().Query()
            .Include(x => x.InstallmentPlan).ThenInclude(p => p.SaleAgreement).ThenInclude(a => a.Booking).ThenInclude(b => b.Customer)
            .Include(x => x.InstallmentPlan).ThenInclude(p => p.SaleAgreement).ThenInclude(a => a.Booking).ThenInclude(b => b.CollectionOfficer)
            .Include(x => x.InstallmentPlan).ThenInclude(p => p.SaleAgreement).ThenInclude(a => a.Booking).ThenInclude(b => b.PropertyUnit).ThenInclude(u => u!.Floor).ThenInclude(fl => fl.Tower).ThenInclude(t => t.Building).ThenInclude(bl => bl.Project)
            .Where(x => x.Status != InstallmentStatus.Paid)
            .AsQueryable();

        if (f.ProjectId.HasValue)
        {
            q = q.Where(x => x.InstallmentPlan.SaleAgreement.Booking.PropertyUnit.Floor.Tower.Building.ProjectId == f.ProjectId.Value);
        }

        if (f.DateFrom.HasValue) q = q.Where(x => x.DueDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.DueDate <= f.DateTo);

        var items = await q.OrderBy(x => x.DueDate).ToListAsync(cancellationToken);
        items = items.Where(x => x.DueAmount + x.PenaltyAmount - x.PaidAmount > 0).ToList();

        if (!string.IsNullOrWhiteSpace(f.Search))
        {
            var search = f.Search.Trim();
            items = items.Where(x =>
                (x.InstallmentPlan.SaleAgreement.Booking.Customer.FullName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.InstallmentPlan.SaleAgreement.Booking.PropertyUnit?.UnitNumber?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();
        }

        return items;
    }

    public async Task<PagedResult<ReportRow>> QueryAsync(IUnitOfWork unitOfWork, ReportFilter filter, int page, int pageSize, CancellationToken cancellationToken)
    {
        var all = await BaseQuery(unitOfWork, filter, cancellationToken);
        var total = all.Count;
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return new PagedResult<ReportRow>(items.Select(ToRow).ToList(), total, page, pageSize);
    }

    public async Task<IReadOnlyList<ReportRow>> QueryAllAsync(IUnitOfWork unitOfWork, ReportFilter filter, CancellationToken cancellationToken)
    {
        var items = await BaseQuery(unitOfWork, filter, cancellationToken);
        return items.Take(10_000).Select(ToRow).ToList();
    }

    private static ReportRow ToRow(Installment x)
    {
        var today = DateTime.UtcNow.Date;
        var booking = x.InstallmentPlan?.SaleAgreement?.Booking;
        var outstanding = x.DueAmount + x.PenaltyAmount - x.PaidAmount;
        var daysOverdue = x.DueDate < today ? (int)(today - x.DueDate).TotalDays : 0;
        var bucket = x.DueDate < today ? "Overdue" : (x.DueDate < today.AddMonths(1) ? "Current" : "Future");

        return new ReportRow(new Dictionary<string, string?>
        {
            ["CustomerName"] = booking?.Customer?.FullName,
            ["Phone"] = booking?.Customer?.Phone,
            ["ProjectName"] = booking?.PropertyUnit?.Floor?.Tower?.Building?.Project?.Name,
            ["UnitNumber"] = booking?.PropertyUnit?.UnitNumber,
            ["DueDate"] = x.DueDate.ToString("yyyy-MM-dd"),
            ["OutstandingAmount"] = outstanding.ToString("N2", CultureInfo.InvariantCulture),
            ["DaysOverdue"] = daysOverdue.ToString(CultureInfo.InvariantCulture),
            ["Bucket"] = bucket,
            ["Rescheduled"] = x.IsRescheduled ? "Yes" : "No",
            ["CollectionOfficer"] = booking?.CollectionOfficer?.FullName,
        });
    }
}
