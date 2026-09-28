using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CashPoBills;

public enum CashPoBillBuildResult
{
    Success,
    OrderNotBillable,
    InvalidLine,
    OverBilled
}

// Shared by create/update: validates the Cash PO is billable, checks each line against the
// quantity not yet billed, and builds detail rows priced from the Cash PO (not from the client).
public static class CashPoBillLineBuilder
{
    public static readonly PurchaseOrderStatus[] BillableStatuses =
    {
        PurchaseOrderStatus.Approved, PurchaseOrderStatus.PartiallyReceived, PurchaseOrderStatus.Received
    };

    public record BuildOutput(CashPoBillBuildResult Result, List<CashPoBillDetail> Details, decimal Total, long RequesterEmployeeId, long ProjectId);

    public static async Task<BuildOutput> BuildAsync(
        IUnitOfWork unitOfWork, SaveCashPoBillDto dto, long? excludeBillId, CancellationToken ct)
    {
        var order = await unitOfWork.Repository<CashPurchaseOrder>().Query()
            .Include(o => o.Details)
            .Include(o => o.CashRequisition)
            .FirstOrDefaultAsync(o => o.Id == dto.CashPurchaseOrderId, ct);

        if (order is null || !order.IsActive || !BillableStatuses.Contains(order.Status))
        {
            return new(CashPoBillBuildResult.OrderNotBillable, new(), 0, 0, 0);
        }

        var billed = await BilledQuantitiesAsync(unitOfWork, order.Id, excludeBillId, ct);

        var details = new List<CashPoBillDetail>();
        decimal total = 0;
        foreach (var line in dto.Details.GroupBy(l => l.CashPurchaseOrderDetailId).Select(g => new { Id = g.Key, Qty = g.Sum(x => x.BilledQuantity) }))
        {
            var poLine = order.Details.FirstOrDefault(d => d.Id == line.Id);
            if (poLine is null || line.Qty <= 0)
            {
                return new(CashPoBillBuildResult.InvalidLine, new(), 0, 0, 0);
            }

            billed.TryGetValue(poLine.Id, out var alreadyBilled);
            if (line.Qty > poLine.OrderedQuantity - alreadyBilled)
            {
                return new(CashPoBillBuildResult.OverBilled, new(), 0, 0, 0);
            }

            var amounts = LineItemCalculator.Calculate(line.Qty, poLine.UnitPrice, poLine.DiscountPercent, poLine.VatPercent, poLine.TaxPercent);
            details.Add(new CashPoBillDetail
            {
                CashPurchaseOrderDetailId = poLine.Id,
                MaterialId = poLine.MaterialId,
                BilledQuantity = line.Qty,
                UnitOfMeasure = poLine.UnitOfMeasure,
                UnitPrice = poLine.UnitPrice,
                DiscountPercent = poLine.DiscountPercent,
                DiscountAmount = amounts.DiscountAmount,
                VatPercent = poLine.VatPercent,
                VatAmount = amounts.VatAmount,
                TaxPercent = poLine.TaxPercent,
                TaxAmount = amounts.TaxAmount,
                LineTotal = amounts.NetAmount
            });
            total += amounts.NetAmount;
        }

        return new(CashPoBillBuildResult.Success, details, total, order.CashRequisition.RequesterEmployeeId, order.CashRequisition.ProjectId);
    }

    public static async Task<Dictionary<long, decimal>> BilledQuantitiesAsync(
        IUnitOfWork unitOfWork, long cashPurchaseOrderId, long? excludeBillId, CancellationToken ct)
    {
        return await unitOfWork.Repository<CashPoBillDetail>().Query()
            .Where(d => d.CashPoBill.CashPurchaseOrderId == cashPurchaseOrderId
                        && d.CashPoBill.IsActive
                        && d.CashPoBill.Status != PoBillStatus.Cancelled
                        && (excludeBillId == null || d.CashPoBillId != excludeBillId))
            .GroupBy(d => d.CashPurchaseOrderDetailId)
            .Select(g => new { g.Key, Qty = g.Sum(x => x.BilledQuantity) })
            .ToDictionaryAsync(x => x.Key, x => x.Qty, ct);
    }
}
