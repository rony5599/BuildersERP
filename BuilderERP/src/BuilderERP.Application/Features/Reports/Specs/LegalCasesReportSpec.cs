using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class LegalCasesReportSpec : IReportSpec
{
    public string Key => "legal-cases";
    public string Name => "Legal Cases Report";
    public string Category => "Legal Module";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("CaseNumber", "Case #"),
        new ReportColumn("CaseTitle", "Title"),
        new ReportColumn("CourtName", "Court"),
        new ReportColumn("CaseType", "Type"),
        new ReportColumn("Project", "Project"),
        new ReportColumn("FilingDate", "Filing Date"),
        new ReportColumn("NextHearingDate", "Next Hearing"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<LegalCase> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<LegalCase>().Query().Include(x => x.Project).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.CaseNumber.Contains(f.Search) || x.CaseTitle.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.FilingDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.FilingDate <= f.DateTo);
        if (f.ProjectId.HasValue) q = q.Where(x => x.ProjectId == f.ProjectId);
        return q.OrderByDescending(x => x.FilingDate);
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

    private static ReportRow ToRow(LegalCase x) => new(new Dictionary<string, string?>
    {
        ["CaseNumber"] = x.CaseNumber,
        ["CaseTitle"] = x.CaseTitle,
        ["CourtName"] = x.CourtName,
        ["CaseType"] = x.CaseType.ToString(),
        ["Project"] = x.Project?.Name,
        ["FilingDate"] = x.FilingDate.ToString("yyyy-MM-dd"),
        ["NextHearingDate"] = x.NextHearingDate?.ToString("yyyy-MM-dd"),
        ["Status"] = x.Status.ToString(),
    });
}
