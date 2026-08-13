namespace BuilderERP.Domain.Entities;

public class VisitorLog : BaseEntity
{
    public string VisitorName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? PurposeOfVisit { get; set; }
    public string? HostName { get; set; }
    public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
    public DateTime? CheckOutTime { get; set; }
    public string? IdProofNumber { get; set; }
    public string? VehicleNumber { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
