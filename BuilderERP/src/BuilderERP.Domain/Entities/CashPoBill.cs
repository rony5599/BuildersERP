using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class CashPoBill : BaseEntity
{
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; } = DateTime.UtcNow;
    public string? MemoNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Remarks { get; set; }
    public PoBillStatus Status { get; set; } = PoBillStatus.Draft;
    public bool IsActive { get; set; } = true;

    public long CashPurchaseOrderId { get; set; }
    public CashPurchaseOrder CashPurchaseOrder { get; set; } = null!;

    // Denormalized from the CPO's requisition so the requester ledger can filter without deep joins.
    public long RequesterEmployeeId { get; set; }
    public Employee RequesterEmployee { get; set; } = null!;

    public ICollection<CashPoBillDetail> Details { get; set; } = new List<CashPoBillDetail>();
}
