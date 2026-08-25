using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class PunchList : BaseEntity
{
    public string ItemNumber { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string Description { get; set; } = string.Empty;
    public PunchListStatus Status { get; set; } = PunchListStatus.Open;
    public string? AssignedTo { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public bool IsActive { get; set; } = true;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
