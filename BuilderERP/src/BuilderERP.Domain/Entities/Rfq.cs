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

    public ICollection<RfqVendor> RfqVendors { get; set; } = new List<RfqVendor>();
    public ICollection<RfqDetail> Details { get; set; } = new List<RfqDetail>();
    public ICollection<VendorQuotation> VendorQuotations { get; set; } = new List<VendorQuotation>();
}
