namespace BuilderERP.Application.DTOs;

public class DailyProgressDto
{
    public long Id { get; set; }
    public DateTime ProgressDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal PercentComplete { get; set; }
    public int ManpowerCount { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateDailyProgressDto
{
    public DateTime ProgressDate { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public decimal PercentComplete { get; set; }
    public int ManpowerCount { get; set; }
    public string? Remarks { get; set; }
    public long ProjectId { get; set; }
}

public class UpdateDailyProgressDto
{
    public long Id { get; set; }
    public DateTime ProgressDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal PercentComplete { get; set; }
    public int ManpowerCount { get; set; }
    public string? Remarks { get; set; }
    public long ProjectId { get; set; }
}
