using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class RiskAssessmentDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime AssessmentDate { get; set; }
    public string AssessedBy { get; set; } = string.Empty;
    public string HazardDescription { get; set; } = string.Empty;
    public RiskLevel RiskLevel { get; set; }
    public string? MitigationMeasures { get; set; }
    public DateTime? ReviewDate { get; set; }
    public bool IsActive { get; set; }
}

public class CreateRiskAssessmentDto
{
    public Guid ProjectId { get; set; }
    public DateTime AssessmentDate { get; set; } = DateTime.UtcNow;
    public string AssessedBy { get; set; } = string.Empty;
    public string HazardDescription { get; set; } = string.Empty;
    public RiskLevel RiskLevel { get; set; } = RiskLevel.Low;
    public string? MitigationMeasures { get; set; }
    public DateTime? ReviewDate { get; set; }
}

public class UpdateRiskAssessmentDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime AssessmentDate { get; set; }
    public string AssessedBy { get; set; } = string.Empty;
    public string HazardDescription { get; set; } = string.Empty;
    public RiskLevel RiskLevel { get; set; }
    public string? MitigationMeasures { get; set; }
    public DateTime? ReviewDate { get; set; }
}
