using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class DelayEventDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int DelayDays { get; set; }
    public DelayReason Reason { get; set; }
    public DateTime ReportedDate { get; set; }
    public bool IsActive { get; set; }
    public Guid? WbsTaskId { get; set; }
    public string? WbsTaskCode { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateDelayEventDto
{
    public string Description { get; set; } = string.Empty;
    public int DelayDays { get; set; }
    public DelayReason Reason { get; set; } = DelayReason.Other;
    public DateTime ReportedDate { get; set; } = DateTime.UtcNow;
    public Guid? WbsTaskId { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateDelayEventDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int DelayDays { get; set; }
    public DelayReason Reason { get; set; }
    public DateTime ReportedDate { get; set; }
    public Guid? WbsTaskId { get; set; }
    public Guid ProjectId { get; set; }
}
