using System.Globalization;
using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class InstallmentsReportSpec : IReportSpec
{
    public string Key => "installments";
    public string Name => "Installments Report";
    public string Category => "Installments";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("AgreementNumber", "Agreement #"),
        new ReportColumn("InstallmentNumber", "Installment #"),
        new ReportColumn("DueDate", "Due Date"),
        new ReportColumn("DueAmount", "Due Amount"),
        new ReportColumn("PenaltyAmount", "Penalty"),
        new ReportColumn("PaidAmount", "Paid"),
        new ReportColumn("Status", "Status"),
    };

    private static IQueryable<Installment> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<Installment>().Query()
            .Include(x => x.InstallmentPlan).ThenInclude(p => p.SaleAgreement)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.InstallmentPlan.SaleAgreement.AgreementNumber.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.DueDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.DueDate <= f.DateTo);
        return q.OrderByDescending(x => x.DueDate);
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

    private static ReportRow ToRow(Installment x) => new(new Dictionary<string, string?>
    {
        ["AgreementNumber"] = x.InstallmentPlan?.SaleAgreement?.AgreementNumber,
        ["InstallmentNumber"] = x.InstallmentNumber.ToString(CultureInfo.InvariantCulture),
        ["DueDate"] = x.DueDate.ToString("yyyy-MM-dd"),
        ["DueAmount"] = x.DueAmount.ToString("N2", CultureInfo.InvariantCulture),
        ["PenaltyAmount"] = x.PenaltyAmount.ToString("N2", CultureInfo.InvariantCulture),
        ["PaidAmount"] = x.PaidAmount.ToString("N2", CultureInfo.InvariantCulture),
        ["Status"] = x.Status.ToString(),
    });
}
