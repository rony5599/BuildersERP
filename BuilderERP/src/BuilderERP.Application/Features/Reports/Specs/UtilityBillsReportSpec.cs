using System.Globalization;
using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class UtilityBillsReportSpec : IReportSpec
{
    public string Key => "utility-bills";
    public string Name => "Utility Bills Report";
    public string Category => "Facility Management";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("BillNumber", "Bill #"),
        new ReportColumn("PropertyUnit", "Unit"),
        new ReportColumn("UtilityType", "Type"),
        new ReportColumn("BillingMonth", "Billing Month"),
        new ReportColumn("Amount", "Amount"),
        new ReportColumn("DueDate", "Due Date"),
        new ReportColumn("PaidDate", "Paid Date"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<UtilityBill> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<UtilityBill>().Query().Include(x => x.PropertyUnit).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.BillNumber.Contains(f.Search) || x.PropertyUnit.UnitNumber.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.DueDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.DueDate <= f.DateTo);
        return q.OrderByDescending(x => x.DueDate);
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

    private static ReportRow ToRow(UtilityBill x) => new(new Dictionary<string, string?>
    {
        ["BillNumber"] = x.BillNumber,
        ["PropertyUnit"] = x.PropertyUnit?.UnitNumber,
        ["UtilityType"] = x.UtilityType.ToString(),
        ["BillingMonth"] = x.BillingMonth.ToString("yyyy-MM"),
        ["Amount"] = x.Amount.ToString("N2", CultureInfo.InvariantCulture),
        ["DueDate"] = x.DueDate.ToString("yyyy-MM-dd"),
        ["PaidDate"] = x.PaidDate?.ToString("yyyy-MM-dd"),
        ["Status"] = x.Status.ToString(),
    });
}
