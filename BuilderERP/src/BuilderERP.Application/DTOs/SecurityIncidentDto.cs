using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class SecurityIncidentDto
{
    public Guid Id { get; set; }
    public string IncidentNumber { get; set; } = string.Empty;
    public SecurityIncidentType IncidentType { get; set; }
    public string? Location { get; set; }
    public DateTime IncidentDateTime { get; set; }
    public string? ReportedBy { get; set; }
    public IncidentSeverity Severity { get; set; }
    public SecurityIncidentStatus Status { get; set; }
    public string? ActionTaken { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsActive { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateSecurityIncidentDto
{
    public string IncidentNumber { get; set; } = string.Empty;
    public SecurityIncidentType IncidentType { get; set; }
    public string? Location { get; set; }
    public DateTime IncidentDateTime { get; set; }
    public string? ReportedBy { get; set; }
    public IncidentSeverity Severity { get; set; }
    public SecurityIncidentStatus Status { get; set; }
    public string? ActionTaken { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateSecurityIncidentDto
{
    public Guid Id { get; set; }
    public string IncidentNumber { get; set; } = string.Empty;
    public SecurityIncidentType IncidentType { get; set; }
    public string? Location { get; set; }
    public DateTime IncidentDateTime { get; set; }
    public string? ReportedBy { get; set; }
    public IncidentSeverity Severity { get; set; }
    public SecurityIncidentStatus Status { get; set; }
    public string? ActionTaken { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}
