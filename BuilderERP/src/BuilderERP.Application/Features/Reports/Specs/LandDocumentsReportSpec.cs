using System.Globalization;
using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class LandDocumentsReportSpec : IReportSpec
{
    public string Key => "land-documents";
    public string Name => "Land Documents Report";
    public string Category => "Legal Module";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("DocumentNumber", "Doc #"),
        new ReportColumn("Title", "Title"),
        new ReportColumn("LandDocumentType", "Type"),
        new ReportColumn("MouzaName", "Mouza"),
        new ReportColumn("KhatianNumber", "Khatian #"),
        new ReportColumn("DagNumber", "Dag #"),
        new ReportColumn("AreaInDecimal", "Area (Dec)"),
        new ReportColumn("Project", "Project"),
        new ReportColumn("AcquisitionDate", "Acquisition Date"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<LandDocument> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<LandDocument>().Query().Include(x => x.Project).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.DocumentNumber.Contains(f.Search) || x.Title.Contains(f.Search) || (x.MouzaName != null && x.MouzaName.Contains(f.Search)));
        if (f.DateFrom.HasValue) q = q.Where(x => x.AcquisitionDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.AcquisitionDate <= f.DateTo);
        if (f.ProjectId.HasValue) q = q.Where(x => x.ProjectId == f.ProjectId);
        return q.OrderByDescending(x => x.AcquisitionDate);
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

    private static ReportRow ToRow(LandDocument x) => new(new Dictionary<string, string?>
    {
        ["DocumentNumber"] = x.DocumentNumber,
        ["Title"] = x.Title,
        ["LandDocumentType"] = x.LandDocumentType.ToString(),
        ["MouzaName"] = x.MouzaName,
        ["KhatianNumber"] = x.KhatianNumber,
        ["DagNumber"] = x.DagNumber,
        ["AreaInDecimal"] = x.AreaInDecimal.ToString("N2", CultureInfo.InvariantCulture),
        ["Project"] = x.Project?.Name,
        ["AcquisitionDate"] = x.AcquisitionDate?.ToString("yyyy-MM-dd"),
        ["Status"] = x.IsActive ? "Active" : "Inactive",
    });
}
