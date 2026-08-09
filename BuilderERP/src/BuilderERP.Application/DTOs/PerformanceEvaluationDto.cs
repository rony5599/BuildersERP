namespace BuilderERP.Application.DTOs;

public class PerformanceEvaluationDto
{
    public Guid Id { get; set; }
    public Guid ContractorId { get; set; }
    public string ContractorName { get; set; } = string.Empty;
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public DateTime EvaluationDate { get; set; }
    public int QualityScore { get; set; }
    public int TimelinessScore { get; set; }
    public int SafetyScore { get; set; }
    public decimal OverallScore { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
}

public class CreatePerformanceEvaluationDto
{
    public Guid ContractorId { get; set; }
    public Guid? ProjectId { get; set; }
    public DateTime EvaluationDate { get; set; } = DateTime.UtcNow;
    public int QualityScore { get; set; }
    public int TimelinessScore { get; set; }
    public int SafetyScore { get; set; }
    public string? Remarks { get; set; }
}

public class UpdatePerformanceEvaluationDto
{
    public Guid Id { get; set; }
    public Guid ContractorId { get; set; }
    public Guid? ProjectId { get; set; }
    public DateTime EvaluationDate { get; set; }
    public int QualityScore { get; set; }
    public int TimelinessScore { get; set; }
    public int SafetyScore { get; set; }
    public string? Remarks { get; set; }
}
