namespace BuilderERP.Domain.Entities;

// Part of a payment head claimed on one bill. ClaimPercent is a share of the contract
// (e.g. 9 = half of an 18% head); across a work order's bills it never exceeds HeadPercent.
public class EwoBillHead : BaseEntity
{
    public string HeadName { get; set; } = string.Empty;
    public decimal HeadPercent { get; set; }
    public decimal ClaimPercent { get; set; }

    public long EwoBillId { get; set; }
    public EwoBill EwoBill { get; set; } = null!;

    public long EngineerWorkOrderPaymentHeadId { get; set; }
    public EngineerWorkOrderPaymentHead EngineerWorkOrderPaymentHead { get; set; } = null!;
}
