using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class VendorQuotation : BaseEntity
{
    public string QuotationNumber { get; set; } = string.Empty;
    public DateTime QuotationDate { get; set; } = DateTime.UtcNow;
    public decimal QuotedAmount { get; set; }
    public int DeliveryDays { get; set; }
    public VendorQuotationStatus Status { get; set; } = VendorQuotationStatus.Received;
    public bool IsActive { get; set; } = true;

    public long RfqId { get; set; }
    public Rfq Rfq { get; set; } = null!;

    public long SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public ICollection<VendorQuotationDetail> Details { get; set; } = new List<VendorQuotationDetail>();
}
