using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class EquipmentRentalDto
{
    public Guid Id { get; set; }
    public Guid EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime RentalStartDate { get; set; }
    public DateTime? RentalEndDate { get; set; }
    public decimal RatePerDay { get; set; }
    public decimal TotalAmount { get; set; }
    public RentalStatus Status { get; set; }
    public bool IsActive { get; set; }
}

public class CreateEquipmentRentalDto
{
    public Guid EquipmentId { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime RentalStartDate { get; set; } = DateTime.UtcNow;
    public DateTime? RentalEndDate { get; set; }
    public decimal RatePerDay { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.Active;
}

public class UpdateEquipmentRentalDto
{
    public Guid Id { get; set; }
    public Guid EquipmentId { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime RentalStartDate { get; set; }
    public DateTime? RentalEndDate { get; set; }
    public decimal RatePerDay { get; set; }
    public RentalStatus Status { get; set; }
}
