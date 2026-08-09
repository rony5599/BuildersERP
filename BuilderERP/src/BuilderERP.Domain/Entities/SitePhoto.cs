namespace BuilderERP.Domain.Entities;

public class SitePhoto : BaseEntity
{
    public string FilePath { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime TakenDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid? DailyProgressId { get; set; }
    public DailyProgress? DailyProgress { get; set; }
}
