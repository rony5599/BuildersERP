using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class ApartmentMaintenanceDto
{
    public Guid Id { get; set; }
    public string MaintenanceNumber { get; set; } = string.Empty;
    public FacilityMaintenanceType MaintenanceType { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal Cost { get; set; }
    public FacilityMaintenanceStatus Status { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
    public bool IsActive { get; set; }
    public string PropertyUnitName { get; set; } = string.Empty;
}

public class CreateApartmentMaintenanceDto
{
    public string MaintenanceNumber { get; set; } = string.Empty;
    public FacilityMaintenanceType MaintenanceType { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal Cost { get; set; }
    public FacilityMaintenanceStatus Status { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
}

public class UpdateApartmentMaintenanceDto
{
    public Guid Id { get; set; }
    public string MaintenanceNumber { get; set; } = string.Empty;
    public FacilityMaintenanceType MaintenanceType { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal Cost { get; set; }
    public FacilityMaintenanceStatus Status { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
}
