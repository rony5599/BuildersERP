using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class MaintenanceRequestDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public MaintenanceRequestType RequestType { get; set; }
    public string Description { get; set; } = string.Empty;
    public RequestPriority Priority { get; set; }
    public MaintenanceRequestStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
    public bool IsActive { get; set; }
    public string PropertyUnitName { get; set; } = string.Empty;
}

public class CreateMaintenanceRequestDto
{
    public string RequestNumber { get; set; } = string.Empty;
    public MaintenanceRequestType RequestType { get; set; }
    public string Description { get; set; } = string.Empty;
    public RequestPriority Priority { get; set; }
    public MaintenanceRequestStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
}

public class UpdateMaintenanceRequestDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public MaintenanceRequestType RequestType { get; set; }
    public string Description { get; set; } = string.Empty;
    public RequestPriority Priority { get; set; }
    public MaintenanceRequestStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
}
