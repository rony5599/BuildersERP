namespace BuilderERP.Application.DTOs;

public class PerformanceEvaluationDto
{
    public long Id { get; set; }
    public long ContractorId { get; set; }
    public string ContractorName { get; set; } = string.Empty;
    public long? ProjectId { get; set; }
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
    public long ContractorId { get; set; }
    public long? ProjectId { get; set; }
    public DateTime EvaluationDate { get; set; } = DateTime.UtcNow;
    public int QualityScore { get; set; }
    public int TimelinessScore { get; set; }
    public int SafetyScore { get; set; }
    public string? Remarks { get; set; }
}

public class UpdatePerformanceEvaluationDto
{
    public long Id { get; set; }
    public long ContractorId { get; set; }
    public long? ProjectId { get; set; }
    public DateTime EvaluationDate { get; set; }
    public int QualityScore { get; set; }
    public int TimelinessScore { get; set; }
    public int SafetyScore { get; set; }
    public string? Remarks { get; set; }
}
