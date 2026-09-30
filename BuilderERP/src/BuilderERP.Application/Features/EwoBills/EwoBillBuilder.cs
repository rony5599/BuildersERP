using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.EngineerWorkOrders;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EwoBills;

public enum EwoBillBuildResult
{
    Success,
    OrderNotBillable,
    NoPaymentHeads,
    PendingDraft,
    InvalidLine,
    OverMeasured,
    OverClaimed,
    NothingPayable
}

// Shared by create/update. Validates the work order is billable, then prices the cumulative
// measurement from the work order's rates and applies the claimed heads cumulatively:
//   CumulativeDue   = MeasuredAmount x (heads claimed by earlier bills + this bill) / 100
//   CertifiedAmount = CumulativeDue - amounts certified by earlier bills
//   NetPayable      = CertifiedAmount + additions - deductions
// so a re-measurement on a later bill also corrects what earlier bills paid.
public static class EwoBillBuilder
{
    public static readonly EngineerWorkOrderStatus[] BillableStatuses =
    {
        EngineerWorkOrderStatus.Approved, EngineerWorkOrderStatus.Active
    };

    public record BuildOutput(
        EwoBillBuildResult Result,
        EwoBill? Values = null,
        List<EwoBillDetail>? Details = null,
        List<EwoBillHead>? Heads = null,
        List<EwoBillAdjustment>? Adjustments = null,
        long ProjectId = 0);

    // Totals of the earlier (non-cancelled, active) bills of one work order chain.
    public record PriorBills(
        IReadOnlyList<EwoBill> Bills,
        decimal CumulativePercent,
        decimal Certified,
        Dictionary<string, decimal> ClaimedByHead,
        string? PendingDraftBillNumber);

    public static async Task<BuildOutput> BuildAsync(IUnitOfWork unitOfWork, SaveEwoBillDto dto, long? excludeBillId, CancellationToken ct)
    {
        var order = await unitOfWork.Repository<EngineerWorkOrder>().Query()
            .Include(o => o.Details)
            .Include(o => o.PaymentHeads)
            .Include(o => o.EngineerWorkOrderRequisition)
            .FirstOrDefaultAsync(o => o.Id == dto.EngineerWorkOrderId, ct);

        if (order is null || !order.IsLatestRevision || !BillableStatuses.Contains(order.Status))
        {
            return new(EwoBillBuildResult.OrderNotBillable);
        }

        if (order.PaymentHeads.Count == 0)
        {
            return new(EwoBillBuildResult.NoPaymentHeads);
        }

        var rootId = order.MotherWorkOrderId ?? order.Id;
        var prior = await LoadPriorBillsAsync(unitOfWork, rootId, excludeBillId, ct);
        if (prior.PendingDraftBillNumber is not null)
        {
            return new(EwoBillBuildResult.PendingDraft);
        }

        var details = new List<EwoBillDetail>();
        foreach (var line in dto.Details.GroupBy(l => l.EngineerWorkOrderDetailId).Select(g => new { Id = g.Key, Qty = g.Sum(x => x.MeasuredQuantity) }))
        {
            var orderLine = order.Details.FirstOrDefault(d => d.Id == line.Id);
            if (orderLine is null || line.Qty < 0)
            {
                return new(EwoBillBuildResult.InvalidLine);
            }

            if (line.Qty > orderLine.Qty)
            {
                return new(EwoBillBuildResult.OverMeasured);
            }

            if (line.Qty == 0)
            {
                continue;
            }

            details.Add(new EwoBillDetail
            {
                EngineerWorkOrderDetailId = orderLine.Id,
                MaterialId = orderLine.MaterialId,
                UnitOfMeasure = orderLine.UnitOfMeasure,
                MeasuredQuantity = line.Qty,
                Rate = orderLine.Rate,
                Amount = Math.Round(line.Qty * orderLine.Rate, 2)
            });
        }

        if (details.Count == 0)
        {
            return new(EwoBillBuildResult.InvalidLine);
        }

        var heads = new List<EwoBillHead>();
        foreach (var claim in dto.Heads.GroupBy(h => h.EngineerWorkOrderPaymentHeadId).Select(g => new { Id = g.Key, Percent = g.Sum(x => x.ClaimPercent) }))
        {
            if (claim.Percent == 0)
            {
                continue;
            }

            var head = order.PaymentHeads.FirstOrDefault(h => h.Id == claim.Id);
            if (head is null || claim.Percent < 0 || decimal.Round(claim.Percent, 2) != claim.Percent)
            {
                return new(EwoBillBuildResult.InvalidLine);
            }

            prior.ClaimedByHead.TryGetValue(EngineerWorkOrderPaymentHeads.Key(head.HeadName), out var claimed);
            if (claim.Percent > head.Percent - claimed)
            {
                return new(EwoBillBuildResult.OverClaimed);
            }

            heads.Add(new EwoBillHead
            {
                EngineerWorkOrderPaymentHeadId = head.Id,
                HeadName = head.HeadName,
                HeadPercent = head.Percent,
                ClaimPercent = claim.Percent
            });
        }

        var adjustments = dto.Adjustments
            .Where(a => a.Amount > 0)
            .Select(a => new EwoBillAdjustment { Type = a.Type, Description = a.Description.Trim(), Amount = Math.Round(a.Amount, 2) })
            .ToList();

        var values = Calculate(
            details.Sum(d => d.Amount),
            prior.CumulativePercent,
            heads.Sum(h => h.ClaimPercent),
            prior.Certified,
            adjustments.Where(a => a.Type == EwoBillAdjustmentType.Addition).Sum(a => a.Amount),
            adjustments.Where(a => a.Type == EwoBillAdjustmentType.Deduction).Sum(a => a.Amount));

        if (values.NetPayable <= 0)
        {
            return new(EwoBillBuildResult.NothingPayable);
        }

        values.EngineerWorkOrderId = order.Id;
        values.RootWorkOrderId = rootId;
        values.SupplierId = order.SupplierId;

        return new(EwoBillBuildResult.Success, values, details, heads, adjustments, order.EngineerWorkOrderRequisition.ProjectId);
    }

    // The bill amounts, as a pure function of the measurement, the claimed percents and the earlier bills.
    public static EwoBill Calculate(decimal measuredAmount, decimal priorPercent, decimal claimPercent, decimal priorCertified, decimal additions, decimal deductions)
    {
        var cumulativePercent = priorPercent + claimPercent;
        var cumulativeDue = Math.Round(measuredAmount * cumulativePercent / 100, 2);
        var certified = cumulativeDue - priorCertified;
        return new EwoBill
        {
            MeasuredAmount = measuredAmount,
            CumulativePercent = cumulativePercent,
            CumulativeDue = cumulativeDue,
            PreviouslyCertified = priorCertified,
            CertifiedAmount = certified,
            AdditionAmount = additions,
            DeductionAmount = deductions,
            NetPayable = certified + additions - deductions
        };
    }

    public static async Task<PriorBills> LoadPriorBillsAsync(IUnitOfWork unitOfWork, long rootWorkOrderId, long? excludeBillId, CancellationToken ct)
    {
        var bills = await unitOfWork.Repository<EwoBill>().Query()
            .Include(b => b.Heads)
            .Include(b => b.Details)
            .Where(b => b.RootWorkOrderId == rootWorkOrderId
                        && b.IsActive
                        && b.Status != PoBillStatus.Cancelled
                        && (excludeBillId == null || b.Id != excludeBillId))
            .OrderBy(b => b.Id)
            .ToListAsync(ct);

        var claimed = bills.SelectMany(b => b.Heads)
            .GroupBy(h => EngineerWorkOrderPaymentHeads.Key(h.HeadName))
            .ToDictionary(g => g.Key, g => g.Sum(h => h.ClaimPercent));

        return new PriorBills(
            bills,
            bills.SelectMany(b => b.Heads).Sum(h => h.ClaimPercent),
            bills.Sum(b => b.CertifiedAmount),
            claimed,
            bills.FirstOrDefault(b => b.Status == PoBillStatus.Draft)?.BillNumber);
    }
}
