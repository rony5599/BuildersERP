using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

// Running bill against an engineer work order, calculated cumulatively:
// CertifiedAmount = MeasuredAmount x CumulativePercent - PreviouslyCertified,
// NetPayable = CertifiedAmount + AdditionAmount - DeductionAmount.
// The amounts are a snapshot taken when the bill is saved; earlier bills are locked, so it stays valid.
public class EwoBill : BaseEntity
{
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; } = DateTime.UtcNow;
    public string? ContractorBillNumber { get; set; }
    public string? MrrNumber { get; set; }
    public string? Remarks { get; set; }
    public PoBillStatus Status { get; set; } = PoBillStatus.Draft;
    public bool IsActive { get; set; } = true;
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }

    public decimal MeasuredAmount { get; set; }
    public decimal CumulativePercent { get; set; }
    public decimal CumulativeDue { get; set; }
    public decimal PreviouslyCertified { get; set; }
    public decimal CertifiedAmount { get; set; }
    public decimal AdditionAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal NetPayable { get; set; }

    // The revision the bill was raised against.
    public long EngineerWorkOrderId { get; set; }
    public EngineerWorkOrder EngineerWorkOrder { get; set; } = null!;

    // First revision of the work order (MotherWorkOrderId ?? Id), so bills of all revisions form one chain.
    public long RootWorkOrderId { get; set; }
    public EngineerWorkOrder RootWorkOrder { get; set; } = null!;

    // Denormalized from the work order so the supplier ledger can filter without deep joins.
    public long SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public ICollection<EwoBillDetail> Details { get; set; } = new List<EwoBillDetail>();
    public ICollection<EwoBillHead> Heads { get; set; } = new List<EwoBillHead>();
    public ICollection<EwoBillAdjustment> Adjustments { get; set; } = new List<EwoBillAdjustment>();
}
