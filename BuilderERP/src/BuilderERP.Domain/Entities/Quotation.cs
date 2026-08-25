using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Quotation : BaseEntity
{
    public decimal QuotedPrice { get; set; }
    public DateTime? ValidUntil { get; set; }
    public QuotationStatus Status { get; set; } = QuotationStatus.Draft;
    public bool IsActive { get; set; } = true;

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public long PropertyUnitId { get; set; }
    public PropertyUnit PropertyUnit { get; set; } = null!;
}
