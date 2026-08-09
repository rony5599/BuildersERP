using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class DrawingApproval : BaseEntity
{
    public Guid DrawingId { get; set; }
    public Drawing Drawing { get; set; } = null!;

    public Guid? DrawingRevisionId { get; set; }
    public DrawingRevision? DrawingRevision { get; set; }

    public string ApproverName { get; set; } = string.Empty;
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public string? Comments { get; set; }
    public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ActionDate { get; set; }
    public bool IsActive { get; set; } = true;
}
