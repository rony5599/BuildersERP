using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class LegalNoticesReportSpec : IReportSpec
{
    public string Key => "legal-notices";
    public string Name => "Legal Notices Report";
    public string Category => "Legal Module";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("NoticeNumber", "Notice #"),
        new ReportColumn("Title", "Title"),
        new ReportColumn("NoticeType", "Type"),
        new ReportColumn("IssuedTo", "Issued To"),
        new ReportColumn("Project", "Project"),
        new ReportColumn("IssueDate", "Issue Date"),
        new ReportColumn("ResponseDeadline", "Response Deadline"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<LegalNotice> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<LegalNotice>().Query().Include(x => x.Project).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.NoticeNumber.Contains(f.Search) || x.Title.Contains(f.Search) || x.IssuedTo.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.IssueDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.IssueDate <= f.DateTo);
        if (f.ProjectId.HasValue) q = q.Where(x => x.ProjectId == f.ProjectId);
        return q.OrderByDescending(x => x.IssueDate);
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

    private static ReportRow ToRow(LegalNotice x) => new(new Dictionary<string, string?>
    {
        ["NoticeNumber"] = x.NoticeNumber,
        ["Title"] = x.Title,
        ["NoticeType"] = x.NoticeType.ToString(),
        ["IssuedTo"] = x.IssuedTo,
        ["Project"] = x.Project?.Name,
        ["IssueDate"] = x.IssueDate.ToString("yyyy-MM-dd"),
        ["ResponseDeadline"] = x.ResponseDeadline?.ToString("yyyy-MM-dd"),
        ["Status"] = x.Status.ToString(),
    });
}
