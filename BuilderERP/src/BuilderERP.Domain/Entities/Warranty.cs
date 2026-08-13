using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Warranty : BaseEntity
{
    public string WarrantyNumber { get; set; } = string.Empty;
    public string ItemCovered { get; set; } = string.Empty;
    public WarrantyType WarrantyType { get; set; } = WarrantyType.Structural;
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; } = DateTime.UtcNow;
    public WarrantyStatus Status { get; set; } = WarrantyStatus.Active;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid PropertyUnitId { get; set; }
    public PropertyUnit PropertyUnit { get; set; } = null!;
}
