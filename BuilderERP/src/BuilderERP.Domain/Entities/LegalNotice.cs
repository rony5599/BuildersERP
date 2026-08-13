using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class LegalNotice : BaseEntity
{
    public string NoticeNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public LegalNoticeType NoticeType { get; set; } = LegalNoticeType.Demand;
    public string IssuedTo { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime? ResponseDeadline { get; set; }
    public LegalNoticeStatus Status { get; set; } = LegalNoticeStatus.Draft;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
