using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class MaintenanceRecordDto
{
    public Guid Id { get; set; }
    public MaintenanceType MaintenanceType { get; set; }
    public DateTime MaintenanceDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public MaintenanceStatus Status { get; set; }
    public DateTime? NextServiceDate { get; set; }
    public bool IsActive { get; set; }
    public Guid EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
}

public class CreateMaintenanceRecordDto
{
    public MaintenanceType MaintenanceType { get; set; } = MaintenanceType.Routine;
    public DateTime MaintenanceDate { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;
    public DateTime? NextServiceDate { get; set; }
    public Guid EquipmentId { get; set; }
}

public class UpdateMaintenanceRecordDto
{
    public Guid Id { get; set; }
    public MaintenanceType MaintenanceType { get; set; }
    public DateTime MaintenanceDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public MaintenanceStatus Status { get; set; }
    public DateTime? NextServiceDate { get; set; }
    public Guid EquipmentId { get; set; }
}
