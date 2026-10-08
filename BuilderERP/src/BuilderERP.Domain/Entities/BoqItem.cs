using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class BoqItem : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; private set; }

    public long BoqId { get; set; }
    public BoqHeader Boq { get; set; } = null!;
    public long WorkGroupId { get; set; }
    public WorkGroup WorkGroup { get; set; } = null!;
}
