namespace BuilderERP.Domain.Entities;

// A payment stage of an engineer work order (e.g. Vertical 18%, Internal 22%, Security 10%).
// The heads of one work order total 100%; EWO bills claim them, fully or partly, as work progresses.
public class EngineerWorkOrderPaymentHead : BaseEntity
{
    public string HeadName { get; set; } = string.Empty;
    public decimal Percent { get; set; }
    public int SortOrder { get; set; }

    public long EngineerWorkOrderId { get; set; }
    public EngineerWorkOrder EngineerWorkOrder { get; set; } = null!;
}
