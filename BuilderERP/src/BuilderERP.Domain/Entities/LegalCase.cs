using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class LegalCase : BaseEntity
{
    public string CaseNumber { get; set; } = string.Empty;
    public string CaseTitle { get; set; } = string.Empty;
    public string? CourtName { get; set; }
    public LegalCaseType CaseType { get; set; } = LegalCaseType.Civil;
    public DateTime FilingDate { get; set; } = DateTime.UtcNow;
    public LegalCaseStatus Status { get; set; } = LegalCaseStatus.Open;
    public string? OpposingParty { get; set; }
    public string? LawyerName { get; set; }
    public DateTime? NextHearingDate { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
