using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class EquipmentDto
{
    public Guid Id { get; set; }
    public string EquipmentCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EquipmentType Type { get; set; }
    public string? Model { get; set; }
    public string? RegistrationNumber { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public EquipmentStatus Status { get; set; }
    public bool IsActive { get; set; }
}

public class CreateEquipmentDto
{
    public string EquipmentCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EquipmentType Type { get; set; } = EquipmentType.Other;
    public string? Model { get; set; }
    public string? RegistrationNumber { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;
}

public class UpdateEquipmentDto
{
    public Guid Id { get; set; }
    public string EquipmentCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EquipmentType Type { get; set; }
    public string? Model { get; set; }
    public string? RegistrationNumber { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public EquipmentStatus Status { get; set; }
}
