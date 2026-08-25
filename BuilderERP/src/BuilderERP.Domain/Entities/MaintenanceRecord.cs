using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class MaintenanceRecord : BaseEntity
{
    public MaintenanceType MaintenanceType { get; set; } = MaintenanceType.Routine;
    public DateTime MaintenanceDate { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;
    public DateTime? NextServiceDate { get; set; }
    public bool IsActive { get; set; } = true;

    public long EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;
}
