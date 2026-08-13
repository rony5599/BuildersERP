using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class LegalAgreementsReportSpec : IReportSpec
{
    public string Key => "legal-agreements";
    public string Name => "Legal Agreements Report";
    public string Category => "Legal Module";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("AgreementNumber", "Agreement #"),
        new ReportColumn("Title", "Title"),
        new ReportColumn("AgreementType", "Type"),
        new ReportColumn("PartyName", "Party"),
        new ReportColumn("Project", "Project"),
        new ReportColumn("EffectiveDate", "Effective Date"),
        new ReportColumn("ExpiryDate", "Expiry Date"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<LegalAgreement> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<LegalAgreement>().Query().Include(x => x.Project).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.AgreementNumber.Contains(f.Search) || x.Title.Contains(f.Search) || x.PartyName.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.EffectiveDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.EffectiveDate <= f.DateTo);
        if (f.ProjectId.HasValue) q = q.Where(x => x.ProjectId == f.ProjectId);
        return q.OrderByDescending(x => x.EffectiveDate);
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

    private static ReportRow ToRow(LegalAgreement x) => new(new Dictionary<string, string?>
    {
        ["AgreementNumber"] = x.AgreementNumber,
        ["Title"] = x.Title,
        ["AgreementType"] = x.AgreementType.ToString(),
        ["PartyName"] = x.PartyName,
        ["Project"] = x.Project?.Name,
        ["EffectiveDate"] = x.EffectiveDate.ToString("yyyy-MM-dd"),
        ["ExpiryDate"] = x.ExpiryDate?.ToString("yyyy-MM-dd"),
        ["Status"] = x.Status.ToString(),
    });
}
