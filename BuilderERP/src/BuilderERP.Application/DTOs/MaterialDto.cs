using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class MaterialDto
{
    public Guid Id { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal ReorderLevel { get; set; }
    public string? Barcode { get; set; }
    public bool IsActive { get; set; }
}

public class CreateMaterialDto
{
    public string MaterialCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal ReorderLevel { get; set; }
    public string? Barcode { get; set; }
}

public class UpdateMaterialDto
{
    public Guid Id { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal ReorderLevel { get; set; }
    public string? Barcode { get; set; }
}
