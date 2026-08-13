using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class CustomersReportSpec : IReportSpec
{
    public string Key => "customers";
    public string Name => "Customers Report";
    public string Category => "CRM";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("FullName", "Name"),
        new ReportColumn("Email", "Email"),
        new ReportColumn("Phone", "Phone"),
        new ReportColumn("NIDNumber", "NID"),
        new ReportColumn("Company", "Company"),
        new ReportColumn("KycStatus", "KYC Status"),
        new ReportColumn("Status", "Status"),
        new ReportColumn("CreatedAt", "Created"),
    };

    private static IQueryable<Customer> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<Customer>().Query().Include(x => x.Company).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.FullName.Contains(f.Search) || x.Email.Contains(f.Search) || x.Phone.Contains(f.Search));
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

    private static ReportRow ToRow(Customer x) => new(new Dictionary<string, string?>
    {
        ["FullName"] = x.FullName,
        ["Email"] = x.Email,
        ["Phone"] = x.Phone,
        ["NIDNumber"] = x.NIDNumber,
        ["Company"] = x.Company?.Name,
        ["KycStatus"] = x.KycStatus.ToString(),
        ["Status"] = x.IsActive ? "Active" : "Inactive",
        ["CreatedAt"] = x.CreatedAt.ToString("yyyy-MM-dd"),
    });
}
