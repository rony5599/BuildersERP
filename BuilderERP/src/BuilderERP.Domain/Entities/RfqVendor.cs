using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class RfqVendor : BaseEntity
{
    public DateTime InvitedDate { get; set; } = DateTime.UtcNow;
    public RfqVendorStatus Status { get; set; } = RfqVendorStatus.Invited;

    public Guid RfqId { get; set; }
    public Rfq Rfq { get; set; } = null!;

    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
}
