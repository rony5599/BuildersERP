using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class WarrantyDto
{
    public Guid Id { get; set; }
    public string WarrantyNumber { get; set; } = string.Empty;
    public string ItemCovered { get; set; } = string.Empty;
    public WarrantyType WarrantyType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public WarrantyStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
    public bool IsActive { get; set; }
    public string PropertyUnitName { get; set; } = string.Empty;
}

public class CreateWarrantyDto
{
    public string WarrantyNumber { get; set; } = string.Empty;
    public string ItemCovered { get; set; } = string.Empty;
    public WarrantyType WarrantyType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public WarrantyStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
}

public class UpdateWarrantyDto
{
    public Guid Id { get; set; }
    public string WarrantyNumber { get; set; } = string.Empty;
    public string ItemCovered { get; set; } = string.Empty;
    public WarrantyType WarrantyType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public WarrantyStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
}
