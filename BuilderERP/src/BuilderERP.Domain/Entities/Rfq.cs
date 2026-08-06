using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Rfq : BaseEntity
{
    public string RfqNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime ClosingDate { get; set; }
    public RfqStatus Status { get; set; } = RfqStatus.Sent;
    public bool IsActive { get; set; } = true;

    public Guid PurchaseRequisitionId { get; set; }
    public PurchaseRequisition PurchaseRequisition { get; set; } = null!;

    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public ICollection<VendorQuotation> VendorQuotations { get; set; } = new List<VendorQuotation>();
}
