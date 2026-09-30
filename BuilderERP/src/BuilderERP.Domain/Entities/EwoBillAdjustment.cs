using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

// Amount added to (extra work, labour) or deducted from (AIT, VAT, advance, company-supplied material) a bill.
public class EwoBillAdjustment : BaseEntity
{
    public EwoBillAdjustmentType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }

    public long EwoBillId { get; set; }
    public EwoBill EwoBill { get; set; } = null!;
}
