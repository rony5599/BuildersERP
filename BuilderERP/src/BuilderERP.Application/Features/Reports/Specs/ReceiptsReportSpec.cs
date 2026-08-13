using System.Globalization;
using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class ReceiptsReportSpec : IReportSpec
{
    public string Key => "receipts";
    public string Name => "Receipts Report";
    public string Category => "Installments";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("ReceiptNumber", "Receipt #"),
        new ReportColumn("AgreementNumber", "Agreement #"),
        new ReportColumn("PaymentDate", "Payment Date"),
        new ReportColumn("AmountPaid", "Amount Paid"),
        new ReportColumn("PaymentMethod", "Method"),
    };

    private static IQueryable<Receipt> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<Receipt>().Query()
            .Include(x => x.Installment).ThenInclude(i => i.InstallmentPlan).ThenInclude(p => p.SaleAgreement)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.ReceiptNumber.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.PaymentDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.PaymentDate <= f.DateTo);
        return q.OrderByDescending(x => x.PaymentDate);
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

    private static ReportRow ToRow(Receipt x) => new(new Dictionary<string, string?>
    {
        ["ReceiptNumber"] = x.ReceiptNumber,
        ["AgreementNumber"] = x.Installment?.InstallmentPlan?.SaleAgreement?.AgreementNumber,
        ["PaymentDate"] = x.PaymentDate.ToString("yyyy-MM-dd"),
        ["AmountPaid"] = x.AmountPaid.ToString("N2", CultureInfo.InvariantCulture),
        ["PaymentMethod"] = x.PaymentMethod.ToString(),
    });
}
