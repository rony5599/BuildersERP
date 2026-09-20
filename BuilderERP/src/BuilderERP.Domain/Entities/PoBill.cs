using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class PoBill : BaseEntity
{
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public string? SupplierInvoiceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Remarks { get; set; }
    public PoBillStatus Status { get; set; } = PoBillStatus.Draft;
    public bool IsActive { get; set; } = true;

    public long PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public ICollection<PoBillDetail> Details { get; set; } = new List<PoBillDetail>();
}
