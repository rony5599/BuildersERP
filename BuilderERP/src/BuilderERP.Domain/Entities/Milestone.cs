using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Milestone : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime TargetDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public MilestoneStatus Status { get; set; } = MilestoneStatus.Pending;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
