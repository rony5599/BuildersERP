using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class PunchListDto
{
    public long Id { get; set; }
    public string ItemNumber { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string Description { get; set; } = string.Empty;
    public PunchListStatus Status { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public bool IsActive { get; set; }
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreatePunchListDto
{
    public string ItemNumber { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string Description { get; set; } = string.Empty;
    public PunchListStatus Status { get; set; } = PunchListStatus.Open;
    public string? AssignedTo { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public long ProjectId { get; set; }
}

public class UpdatePunchListDto
{
    public long Id { get; set; }
    public string ItemNumber { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string Description { get; set; } = string.Empty;
    public PunchListStatus Status { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public long ProjectId { get; set; }
}
