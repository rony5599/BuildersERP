using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class RiskAssessment : BaseEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public DateTime AssessmentDate { get; set; } = DateTime.UtcNow;
    public string AssessedBy { get; set; } = string.Empty;
    public string HazardDescription { get; set; } = string.Empty;
    public RiskLevel RiskLevel { get; set; } = RiskLevel.Low;
    public string? MitigationMeasures { get; set; }
    public DateTime? ReviewDate { get; set; }
    public bool IsActive { get; set; } = true;
}
