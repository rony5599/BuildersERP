using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class IncidentReport : BaseEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public DateTime IncidentDate { get; set; } = DateTime.UtcNow;
    public string ReportedBy { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string Description { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; } = IncidentSeverity.Minor;
    public string? InjuredPersonName { get; set; }
    public IncidentStatus Status { get; set; } = IncidentStatus.Reported;
    public string? CorrectiveAction { get; set; }
    public DateTime? ClosedDate { get; set; }
    public bool IsActive { get; set; } = true;
}
