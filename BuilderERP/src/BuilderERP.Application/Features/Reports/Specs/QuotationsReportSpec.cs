using System.Globalization;
using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class QuotationsReportSpec : IReportSpec
{
    public string Key => "quotations";
    public string Name => "Quotations Report";
    public string Category => "Sales";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("Customer", "Customer"),
        new ReportColumn("PropertyUnit", "Unit"),
        new ReportColumn("QuotedPrice", "Quoted Price"),
        new ReportColumn("ValidUntil", "Valid Until"),
        new ReportColumn("Status", "Status"),
        new ReportColumn("CreatedAt", "Created"),
    };

    private static IQueryable<Quotation> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<Quotation>().Query().Include(x => x.Customer).Include(x => x.PropertyUnit).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.Customer.FullName.Contains(f.Search) || x.PropertyUnit.UnitNumber.Contains(f.Search));
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

    private static ReportRow ToRow(Quotation x) => new(new Dictionary<string, string?>
    {
        ["Customer"] = x.Customer?.FullName,
        ["PropertyUnit"] = x.PropertyUnit?.UnitNumber,
        ["QuotedPrice"] = x.QuotedPrice.ToString("N2", CultureInfo.InvariantCulture),
        ["ValidUntil"] = x.ValidUntil?.ToString("yyyy-MM-dd"),
        ["Status"] = x.Status.ToString(),
        ["CreatedAt"] = x.CreatedAt.ToString("yyyy-MM-dd"),
    });
}
