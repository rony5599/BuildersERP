using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class LegalCaseDto
{
    public Guid Id { get; set; }
    public string CaseNumber { get; set; } = string.Empty;
    public string CaseTitle { get; set; } = string.Empty;
    public string? CourtName { get; set; }
    public LegalCaseType CaseType { get; set; }
    public DateTime FilingDate { get; set; }
    public LegalCaseStatus Status { get; set; }
    public string? OpposingParty { get; set; }
    public string? LawyerName { get; set; }
    public DateTime? NextHearingDate { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsActive { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateLegalCaseDto
{
    public string CaseNumber { get; set; } = string.Empty;
    public string CaseTitle { get; set; } = string.Empty;
    public string? CourtName { get; set; }
    public LegalCaseType CaseType { get; set; }
    public DateTime FilingDate { get; set; }
    public LegalCaseStatus Status { get; set; }
    public string? OpposingParty { get; set; }
    public string? LawyerName { get; set; }
    public DateTime? NextHearingDate { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateLegalCaseDto
{
    public Guid Id { get; set; }
    public string CaseNumber { get; set; } = string.Empty;
    public string CaseTitle { get; set; } = string.Empty;
    public string? CourtName { get; set; }
    public LegalCaseType CaseType { get; set; }
    public DateTime FilingDate { get; set; }
    public LegalCaseStatus Status { get; set; }
    public string? OpposingParty { get; set; }
    public string? LawyerName { get; set; }
    public DateTime? NextHearingDate { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}
