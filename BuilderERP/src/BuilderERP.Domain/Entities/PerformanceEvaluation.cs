namespace BuilderERP.Domain.Entities;

public class PerformanceEvaluation : BaseEntity
{
    public Guid ContractorId { get; set; }
    public Contractor Contractor { get; set; } = null!;

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public DateTime EvaluationDate { get; set; } = DateTime.UtcNow;
    public int QualityScore { get; set; }
    public int TimelinessScore { get; set; }
    public int SafetyScore { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;
}
