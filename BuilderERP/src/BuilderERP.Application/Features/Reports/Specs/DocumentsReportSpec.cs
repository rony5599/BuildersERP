using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class DocumentsReportSpec : IReportSpec
{
    public string Key => "documents";
    public string Name => "Documents Report";
    public string Category => "Document Management";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("DocumentNumber", "Doc #"),
        new ReportColumn("Title", "Title"),
        new ReportColumn("DocumentType", "Type"),
        new ReportColumn("Customer", "Customer"),
        new ReportColumn("Project", "Project"),
        new ReportColumn("IssueDate", "Issue Date"),
        new ReportColumn("ExpiryDate", "Expiry Date"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<Document> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<Document>().Query().Include(x => x.Customer).Include(x => x.Project).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.DocumentNumber.Contains(f.Search) || x.Title.Contains(f.Search));
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

    private static ReportRow ToRow(Document x) => new(new Dictionary<string, string?>
    {
        ["DocumentNumber"] = x.DocumentNumber,
        ["Title"] = x.Title,
        ["DocumentType"] = x.DocumentType.ToString(),
        ["Customer"] = x.Customer?.FullName,
        ["Project"] = x.Project?.Name,
        ["IssueDate"] = x.IssueDate?.ToString("yyyy-MM-dd"),
        ["ExpiryDate"] = x.ExpiryDate?.ToString("yyyy-MM-dd"),
        ["Status"] = x.IsActive ? "Active" : "Inactive",
    });
}
