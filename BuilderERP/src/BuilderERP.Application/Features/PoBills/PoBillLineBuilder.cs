using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PoBills;

public enum PoBillBuildResult
{
    Success,
    PurchaseOrderNotBillable,
    InvalidLine,
    OverBilled
}

// Shared by create/update: validates the PO is billable, checks each line against the
// quantity not yet billed, and builds detail rows priced from the PO (not from the client).
public static class PoBillLineBuilder
{
    public static readonly PurchaseOrderStatus[] BillableStatuses =
    {
        PurchaseOrderStatus.Approved, PurchaseOrderStatus.PartiallyReceived, PurchaseOrderStatus.Received
    };

    public static async Task<(PoBillBuildResult Result, List<PoBillDetail> Details, decimal Total)> BuildAsync(
        IUnitOfWork unitOfWork, SavePoBillDto dto, long? excludeBillId, CancellationToken ct)
    {
        var order = await unitOfWork.Repository<PurchaseOrder>().Query()
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.Id == dto.PurchaseOrderId, ct);

        if (order is null || !order.IsActive || !BillableStatuses.Contains(order.Status))
        {
            return (PoBillBuildResult.PurchaseOrderNotBillable, new(), 0);
        }

        var billed = await BilledQuantitiesAsync(unitOfWork, order.Id, excludeBillId, ct);

        var details = new List<PoBillDetail>();
        decimal total = 0;
        foreach (var line in dto.Details.GroupBy(l => l.PurchaseOrderDetailId).Select(g => new { Id = g.Key, Qty = g.Sum(x => x.BilledQuantity) }))
        {
            var poLine = order.Details.FirstOrDefault(d => d.Id == line.Id);
            if (poLine is null || line.Qty <= 0)
            {
                return (PoBillBuildResult.InvalidLine, new(), 0);
            }

            billed.TryGetValue(poLine.Id, out var alreadyBilled);
            if (line.Qty > poLine.OrderedQuantity - alreadyBilled)
            {
                return (PoBillBuildResult.OverBilled, new(), 0);
            }

            var amounts = LineItemCalculator.Calculate(line.Qty, poLine.UnitPrice, poLine.DiscountPercent, poLine.VatPercent, poLine.TaxPercent);
            details.Add(new PoBillDetail
            {
                PurchaseOrderDetailId = poLine.Id,
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

        return (PoBillBuildResult.Success, details, total);
    }

    public static async Task<Dictionary<long, decimal>> BilledQuantitiesAsync(
        IUnitOfWork unitOfWork, long purchaseOrderId, long? excludeBillId, CancellationToken ct)
    {
        return await unitOfWork.Repository<PoBillDetail>().Query()
            .Where(d => d.PoBill.PurchaseOrderId == purchaseOrderId
                        && d.PoBill.IsActive
                        && d.PoBill.Status != PoBillStatus.Cancelled
                        && d.PoBill.Status != PoBillStatus.Rejected
                        && (excludeBillId == null || d.PoBillId != excludeBillId))
            .GroupBy(d => d.PurchaseOrderDetailId)
            .Select(g => new { g.Key, Qty = g.Sum(x => x.BilledQuantity) })
            .ToDictionaryAsync(x => x.Key, x => x.Qty, ct);
    }
}
