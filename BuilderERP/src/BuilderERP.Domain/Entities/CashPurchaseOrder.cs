using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class CashPurchaseOrder : BaseEntity
{
    public string CPONumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public decimal ReceivedAmount { get; set; }
    public DateTime DeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public bool IsActive { get; set; } = true;
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }

    public string? TermsOfPayment { get; set; }
    public string? DispatchedThrough { get; set; }
    public string? Destination { get; set; }
    public string? Remarks { get; set; }

    public long SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public long CashRequisitionId { get; set; }
    public CashRequisition CashRequisition { get; set; } = null!;

    public ICollection<CashPurchaseOrderDetail> Details { get; set; } = new List<CashPurchaseOrderDetail>();
}
