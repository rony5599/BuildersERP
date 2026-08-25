using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class MaintenanceRequest : BaseEntity
{
    public string RequestNumber { get; set; } = string.Empty;
    public MaintenanceRequestType RequestType { get; set; } = MaintenanceRequestType.Plumbing;
    public string Description { get; set; } = string.Empty;
    public RequestPriority Priority { get; set; } = RequestPriority.Low;
    public MaintenanceRequestStatus Status { get; set; } = MaintenanceRequestStatus.Open;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public long PropertyUnitId { get; set; }
    public PropertyUnit PropertyUnit { get; set; } = null!;
}
