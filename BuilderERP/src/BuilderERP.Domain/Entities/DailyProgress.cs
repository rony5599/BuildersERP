namespace BuilderERP.Domain.Entities;

public class DailyProgress : BaseEntity
{
    public DateTime ProgressDate { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public decimal PercentComplete { get; set; }
    public int ManpowerCount { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
