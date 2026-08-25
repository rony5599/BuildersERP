namespace BuilderERP.Domain.Entities;

public class EngineerWorkOrderDetail : BaseEntity
{
    public string? Description { get; set; }
    public decimal Qty { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string? Remarks { get; set; }

    public Guid EngineerWorkOrderId { get; set; }
    public EngineerWorkOrder EngineerWorkOrder { get; set; } = null!;

    public Guid MaterialId { get; set; }
    public Material Material { get; set; } = null!;
}
