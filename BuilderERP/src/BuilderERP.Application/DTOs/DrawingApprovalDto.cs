using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class DrawingApprovalDto
{
    public Guid Id { get; set; }
    public Guid DrawingId { get; set; }
    public string DrawingNumber { get; set; } = string.Empty;
    public Guid? DrawingRevisionId { get; set; }
    public string? RevisionCode { get; set; }
    public string ApproverName { get; set; } = string.Empty;
    public ApprovalStatus Status { get; set; }
    public string? Comments { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? ActionDate { get; set; }
    public bool IsActive { get; set; }
}

public class CreateDrawingApprovalDto
{
    public Guid DrawingId { get; set; }
    public Guid? DrawingRevisionId { get; set; }
    public string ApproverName { get; set; } = string.Empty;
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public string? Comments { get; set; }
    public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ActionDate { get; set; }
}

public class UpdateDrawingApprovalDto
{
    public Guid Id { get; set; }
    public Guid DrawingId { get; set; }
    public Guid? DrawingRevisionId { get; set; }
    public string ApproverName { get; set; } = string.Empty;
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public string? Comments { get; set; }
    public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ActionDate { get; set; }
}
