using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class BoqItemDto
{
    public long Id { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string? Category { get; set; }
    public bool IsActive { get; set; }
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateBoqItemDto
{
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public string? Category { get; set; }
    public long ProjectId { get; set; }
}

public class UpdateBoqItemDto
{
    public long Id { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public string? Category { get; set; }
    public long ProjectId { get; set; }
}
