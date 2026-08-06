using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Material : BaseEntity
{
    public string MaterialCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal ReorderLevel { get; set; }
    public string? Barcode { get; set; }
    public bool IsActive { get; set; } = true;
}
