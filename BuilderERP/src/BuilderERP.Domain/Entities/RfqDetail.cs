using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class RfqDetail : BaseEntity
{
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public string? Specification { get; set; }

    public Guid RfqId { get; set; }
    public Rfq Rfq { get; set; } = null!;

    public Guid MaterialId { get; set; }
    public Material Material { get; set; } = null!;
}
