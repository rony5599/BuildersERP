using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class LandMutationsReportSpec : IReportSpec
{
    public string Key => "land-mutations";
    public string Name => "Land Mutations Report";
    public string Category => "Legal Module";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("MutationNumber", "Mutation #"),
        new ReportColumn("ApplicantName", "Applicant"),
        new ReportColumn("KhatianNumber", "Khatian #"),
        new ReportColumn("DagNumber", "Dag #"),
        new ReportColumn("Project", "Project"),
        new ReportColumn("MutationDate", "Mutation Date"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<LandMutation> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<LandMutation>().Query().Include(x => x.Project).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.MutationNumber.Contains(f.Search) || x.ApplicantName.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.MutationDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.MutationDate <= f.DateTo);
        if (f.ProjectId.HasValue) q = q.Where(x => x.ProjectId == f.ProjectId);
        return q.OrderByDescending(x => x.MutationDate);
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

    private static ReportRow ToRow(LandMutation x) => new(new Dictionary<string, string?>
    {
        ["MutationNumber"] = x.MutationNumber,
        ["ApplicantName"] = x.ApplicantName,
        ["KhatianNumber"] = x.KhatianNumber,
        ["DagNumber"] = x.DagNumber,
        ["Project"] = x.Project?.Name,
        ["MutationDate"] = x.MutationDate.ToString("yyyy-MM-dd"),
        ["Status"] = x.Status.ToString(),
    });
}
