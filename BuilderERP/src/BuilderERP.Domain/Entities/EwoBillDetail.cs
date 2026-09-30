using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

// Measured work to date (cumulative, not just since the previous bill) for one work order line.
public class EwoBillDetail : BaseEntity
{
    public decimal MeasuredQuantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }

    public long EwoBillId { get; set; }
    public EwoBill EwoBill { get; set; } = null!;

    public long EngineerWorkOrderDetailId { get; set; }
    public EngineerWorkOrderDetail EngineerWorkOrderDetail { get; set; } = null!;

    public long MaterialId { get; set; }
    public Material Material { get; set; } = null!;
}
