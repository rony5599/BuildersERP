using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class LegalNoticeDto
{
    public long Id { get; set; }
    public string NoticeNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public LegalNoticeType NoticeType { get; set; }
    public string IssuedTo { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? ResponseDeadline { get; set; }
    public LegalNoticeStatus Status { get; set; }
    public string? Remarks { get; set; }
    public long ProjectId { get; set; }
    public bool IsActive { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateLegalNoticeDto
{
    public string NoticeNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public LegalNoticeType NoticeType { get; set; }
    public string IssuedTo { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? ResponseDeadline { get; set; }
    public LegalNoticeStatus Status { get; set; }
    public string? Remarks { get; set; }
    public long ProjectId { get; set; }
}

public class UpdateLegalNoticeDto
{
    public long Id { get; set; }
    public string NoticeNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public LegalNoticeType NoticeType { get; set; }
    public string IssuedTo { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? ResponseDeadline { get; set; }
    public LegalNoticeStatus Status { get; set; }
    public string? Remarks { get; set; }
    public long ProjectId { get; set; }
}
