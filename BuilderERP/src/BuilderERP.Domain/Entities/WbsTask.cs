namespace BuilderERP.Domain.Entities;

public class WbsTask : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PercentComplete { get; set; }
    public int Sequence { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid? ParentId { get; set; }
    public WbsTask? Parent { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
