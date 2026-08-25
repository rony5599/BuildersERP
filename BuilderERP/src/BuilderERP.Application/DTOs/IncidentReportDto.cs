using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class IncidentReportDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime IncidentDate { get; set; }
    public string ReportedBy { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string Description { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; }
    public string? InjuredPersonName { get; set; }
    public IncidentStatus Status { get; set; }
    public string? CorrectiveAction { get; set; }
    public DateTime? ClosedDate { get; set; }
    public bool IsActive { get; set; }
}

public class CreateIncidentReportDto
{
    public long ProjectId { get; set; }
    public DateTime IncidentDate { get; set; } = DateTime.UtcNow;
    public string ReportedBy { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string Description { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; } = IncidentSeverity.Minor;
    public string? InjuredPersonName { get; set; }
    public IncidentStatus Status { get; set; } = IncidentStatus.Reported;
    public string? CorrectiveAction { get; set; }
    public DateTime? ClosedDate { get; set; }
}

public class UpdateIncidentReportDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public DateTime IncidentDate { get; set; }
    public string ReportedBy { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string Description { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; }
    public string? InjuredPersonName { get; set; }
    public IncidentStatus Status { get; set; }
    public string? CorrectiveAction { get; set; }
    public DateTime? ClosedDate { get; set; }
}
