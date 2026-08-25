using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class EngineerWorkOrderDetail : BaseEntity
{
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal Qty { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string? Remarks { get; set; }

    public long EngineerWorkOrderId { get; set; }
    public EngineerWorkOrder EngineerWorkOrder { get; set; } = null!;

    public long MaterialId { get; set; }
    public Material Material { get; set; } = null!;
}
