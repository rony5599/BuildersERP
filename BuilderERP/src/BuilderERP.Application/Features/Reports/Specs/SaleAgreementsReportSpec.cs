using System.Globalization;
using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class SaleAgreementsReportSpec : IReportSpec
{
    public string Key => "sale-agreements";
    public string Name => "Sale Agreements Report";
    public string Category => "Sales";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("AgreementNumber", "Agreement #"),
        new ReportColumn("Customer", "Customer"),
        new ReportColumn("PropertyUnit", "Unit"),
        new ReportColumn("AgreementDate", "Agreement Date"),
        new ReportColumn("TotalSalePrice", "Total Price"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<SaleAgreement> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<SaleAgreement>().Query()
            .Include(x => x.Booking).ThenInclude(b => b.Customer)
            .Include(x => x.Booking).ThenInclude(b => b.PropertyUnit)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.AgreementNumber.Contains(f.Search) || x.Booking.Customer.FullName.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.AgreementDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.AgreementDate <= f.DateTo);
        return q.OrderByDescending(x => x.AgreementDate);
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

    private static ReportRow ToRow(SaleAgreement x) => new(new Dictionary<string, string?>
    {
        ["AgreementNumber"] = x.AgreementNumber,
        ["Customer"] = x.Booking?.Customer?.FullName,
        ["PropertyUnit"] = x.Booking?.PropertyUnit?.UnitNumber,
        ["AgreementDate"] = x.AgreementDate.ToString("yyyy-MM-dd"),
        ["TotalSalePrice"] = x.TotalSalePrice.ToString("N2", CultureInfo.InvariantCulture),
        ["Status"] = x.Status.ToString(),
    });
}
