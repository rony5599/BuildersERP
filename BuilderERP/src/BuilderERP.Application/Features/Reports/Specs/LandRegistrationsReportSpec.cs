using System.Globalization;
using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class LandRegistrationsReportSpec : IReportSpec
{
    public string Key => "land-registrations";
    public string Name => "Land Registrations Report";
    public string Category => "Legal Module";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("RegistrationNumber", "Registration #"),
        new ReportColumn("DeedNumber", "Deed #"),
        new ReportColumn("SubRegistryOffice", "Sub Registry Office"),
        new ReportColumn("RegistrationFee", "Fee"),
        new ReportColumn("Project", "Project"),
        new ReportColumn("RegistrationDate", "Registration Date"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<LandRegistration> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<LandRegistration>().Query().Include(x => x.Project).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.RegistrationNumber.Contains(f.Search) || x.DeedNumber.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.RegistrationDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.RegistrationDate <= f.DateTo);
        if (f.ProjectId.HasValue) q = q.Where(x => x.ProjectId == f.ProjectId);
        return q.OrderByDescending(x => x.RegistrationDate);
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

    private static ReportRow ToRow(LandRegistration x) => new(new Dictionary<string, string?>
    {
        ["RegistrationNumber"] = x.RegistrationNumber,
        ["DeedNumber"] = x.DeedNumber,
        ["SubRegistryOffice"] = x.SubRegistryOffice,
        ["RegistrationFee"] = x.RegistrationFee.ToString("N2", CultureInfo.InvariantCulture),
        ["Project"] = x.Project?.Name,
        ["RegistrationDate"] = x.RegistrationDate.ToString("yyyy-MM-dd"),
        ["Status"] = x.Status.ToString(),
    });
}
