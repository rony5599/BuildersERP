using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class MilestoneDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime TargetDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public MilestoneStatus Status { get; set; }
    public string? Remarks { get; set; }
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateMilestoneDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime TargetDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public MilestoneStatus Status { get; set; } = MilestoneStatus.Pending;
    public string? Remarks { get; set; }
    public long ProjectId { get; set; }
}

public class UpdateMilestoneDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime TargetDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public MilestoneStatus Status { get; set; }
    public string? Remarks { get; set; }
    public long ProjectId { get; set; }
}
