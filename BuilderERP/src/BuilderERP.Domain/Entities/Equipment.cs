using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Equipment : BaseEntity
{
    public string EquipmentCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EquipmentType Type { get; set; } = EquipmentType.Other;
    public string? Model { get; set; }
    public string? RegistrationNumber { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;
    public bool IsActive { get; set; } = true;
}
