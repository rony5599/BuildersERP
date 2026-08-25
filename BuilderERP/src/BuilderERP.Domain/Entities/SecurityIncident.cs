using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class SecurityIncident : BaseEntity
{
    public string IncidentNumber { get; set; } = string.Empty;
    public SecurityIncidentType IncidentType { get; set; } = SecurityIncidentType.Theft;
    public string? Location { get; set; }
    public DateTime IncidentDateTime { get; set; } = DateTime.UtcNow;
    public string? ReportedBy { get; set; }
    public IncidentSeverity Severity { get; set; } = IncidentSeverity.Minor;
    public SecurityIncidentStatus Status { get; set; } = SecurityIncidentStatus.Reported;
    public string? ActionTaken { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
