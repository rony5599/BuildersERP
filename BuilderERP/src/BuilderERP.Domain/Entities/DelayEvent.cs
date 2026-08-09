using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class DelayEvent : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public int DelayDays { get; set; }
    public DelayReason Reason { get; set; } = DelayReason.Other;
    public DateTime ReportedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public Guid? WbsTaskId { get; set; }
    public WbsTask? WbsTask { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
