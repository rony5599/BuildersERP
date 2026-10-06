using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class EwoBillDto
{
    public long Id { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; }
    public string? ContractorBillNumber { get; set; }
    public string? MrrNumber { get; set; }
    public string? Remarks { get; set; }
    public PoBillStatus Status { get; set; }
    public bool IsActive { get; set; }
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? PreparedBy { get; set; }
    public long EngineerWorkOrderId { get; set; }
    public long RootWorkOrderId { get; set; }
    public string WorkOrderNo { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public decimal MeasuredAmount { get; set; }
    public decimal CumulativePercent { get; set; }
    public decimal CertifiedAmount { get; set; }
    public decimal AdditionAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal NetPayable { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Outstanding => NetPayable - PaidAmount;
    public string HeadsSummary { get; set; } = string.Empty;
    public List<EwoBillDetailDto> Details { get; set; } = new();
    public List<EwoBillHeadDto> Heads { get; set; } = new();
    public List<EwoBillAdjustmentDto> Adjustments { get; set; } = new();
}

public class EwoBillDetailDto
{
    public long EngineerWorkOrderDetailId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal MeasuredQuantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}

public class EwoBillHeadDto
{
    public long EngineerWorkOrderPaymentHeadId { get; set; }
    public string HeadName { get; set; } = string.Empty;
    public decimal HeadPercent { get; set; }
    public decimal ClaimPercent { get; set; }
}

public class EwoBillAdjustmentDto
{
    public EwoBillAdjustmentType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class EwoBillMeasurementInputDto
{
    public long EngineerWorkOrderDetailId { get; set; }
    public decimal MeasuredQuantity { get; set; }
}

public class EwoBillHeadInputDto
{
    public long EngineerWorkOrderPaymentHeadId { get; set; }
    public decimal ClaimPercent { get; set; }
}

// Used for both create (Id = 0) and edit. Rates and head percents always come from the work order.
public class SaveEwoBillDto
{
    public long Id { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; } = DateTime.UtcNow;
    public string? ContractorBillNumber { get; set; }
    public string? MrrNumber { get; set; }
    public string? Remarks { get; set; }
    public PoBillStatus Status { get; set; } = PoBillStatus.Draft;
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public long EngineerWorkOrderId { get; set; }
    public List<EwoBillMeasurementInputDto> Details { get; set; } = new();
    public List<EwoBillHeadInputDto> Heads { get; set; } = new();
    public List<EwoBillAdjustmentDto> Adjustments { get; set; } = new();
}

public class BillableEngineerWorkOrderDto
{
    public long Id { get; set; }
    public string WorkOrderNo { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
}

// Everything the bill form needs for one work order: its lines, its heads with what earlier
// bills already claimed, and the totals of those earlier bills.
public class EwoBillableDataDto
{
    public long EngineerWorkOrderId { get; set; }
    public string WorkOrderNo { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public decimal ContractAmount { get; set; }
    public decimal PreviousCumulativePercent { get; set; }
    public decimal PreviouslyCertified { get; set; }
    public int PreviousBillCount { get; set; }
    // Set when another bill of this work order is still Draft: a new bill waits until that one is approved or cancelled.
    public string? PendingDraftBillNumber { get; set; }
    public List<EwoBillableLineDto> Lines { get; set; } = new();
    public List<EwoBillableHeadDto> Heads { get; set; } = new();
}

public class EwoBillableLineDto
{
    public long EngineerWorkOrderDetailId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal OrderedQuantity { get; set; }
    public decimal Rate { get; set; }
    public decimal PreviousMeasuredQuantity { get; set; }
}

public class EwoBillableHeadDto
{
    public long EngineerWorkOrderPaymentHeadId { get; set; }
    public string HeadName { get; set; } = string.Empty;
    public decimal Percent { get; set; }
    public decimal ClaimedPercent { get; set; }
    public decimal RemainingPercent => Percent - ClaimedPercent;
}

public class EwoBillPrintDto
{
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; }
    public string? ContractorBillNumber { get; set; }
    public string? MrrNumber { get; set; }
    public string WorkOrderNo { get; set; } = string.Empty;
    public string? ProjectName { get; set; }
    public int BillSequence { get; set; }
    public PoBillStatus Status { get; set; }
    public string? Remarks { get; set; }
    public string? PreparedBy { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyAddress { get; set; }
    public string? CompanyPhone { get; set; }
    public string? CompanyEmail { get; set; }

    public string SupplierName { get; set; } = string.Empty;
    public string? SupplierAddress { get; set; }
    public string? SupplierPhone { get; set; }
    public string? SupplierEmail { get; set; }

    public string? PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }

    public List<EngineerWorkOrderPrintLineDto> Lines { get; set; } = new();
    public List<EwoBillHeadDto> Heads { get; set; } = new();
    public List<EwoBillAdjustmentDto> Adjustments { get; set; } = new();

    public decimal MeasuredAmount { get; set; }
    public decimal CumulativePercent { get; set; }
    public decimal CumulativeDue { get; set; }
    public decimal PreviouslyCertified { get; set; }
    public decimal CertifiedAmount { get; set; }
    public decimal AdditionAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal NetPayable { get; set; }
}

// Contract-wise position of one work order across all its revisions: heads, bills, payments, outstanding.
public class EwoStatementDto
{
    public long RootWorkOrderId { get; set; }
    public string WorkOrderNo { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public decimal ContractAmount { get; set; }
    public decimal LatestMeasuredAmount { get; set; }
    public List<EwoStatementHeadDto> Heads { get; set; } = new();
    public List<EwoBillDto> Bills { get; set; } = new();
    // Drafts are listed but only approved bills are payable.
    public decimal TotalBilled => Bills.Where(b => b.Status == PoBillStatus.Approved).Sum(b => b.NetPayable);
    public decimal TotalPaid => Bills.Sum(b => b.PaidAmount);
    public decimal Outstanding => TotalBilled - TotalPaid;
    public decimal ClaimedPercent => Heads.Sum(h => h.ClaimedPercent);
    // Value of the latest measurement not yet certified because its heads are unclaimed (e.g. security/retention).
    public decimal UnclaimedAmount => Math.Round(LatestMeasuredAmount * (100 - ClaimedPercent) / 100, 2);

    public string? CompanyName { get; set; }
    public string? PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }
}

public class EwoStatementHeadDto
{
    public string HeadName { get; set; } = string.Empty;
    public decimal Percent { get; set; }
    public decimal ClaimedPercent { get; set; }
    public string BillNumbers { get; set; } = string.Empty;
    public decimal RemainingPercent => Percent - ClaimedPercent;
}
