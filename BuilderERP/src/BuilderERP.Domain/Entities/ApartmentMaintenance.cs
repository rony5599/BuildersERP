using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class ApartmentMaintenance : BaseEntity
{
    public string MaintenanceNumber { get; set; } = string.Empty;
    public FacilityMaintenanceType MaintenanceType { get; set; } = FacilityMaintenanceType.Routine;
    public DateTime ScheduledDate { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedDate { get; set; }
    public decimal Cost { get; set; }
    public FacilityMaintenanceStatus Status { get; set; } = FacilityMaintenanceStatus.Scheduled;
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid PropertyUnitId { get; set; }
    public PropertyUnit PropertyUnit { get; set; } = null!;
}
