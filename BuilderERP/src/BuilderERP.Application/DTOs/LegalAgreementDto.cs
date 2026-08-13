using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class LegalAgreementDto
{
    public Guid Id { get; set; }
    public string AgreementNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public LegalAgreementType AgreementType { get; set; }
    public string PartyName { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public LegalAgreementStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsActive { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateLegalAgreementDto
{
    public string AgreementNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public LegalAgreementType AgreementType { get; set; }
    public string PartyName { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public LegalAgreementStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateLegalAgreementDto
{
    public Guid Id { get; set; }
    public string AgreementNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public LegalAgreementType AgreementType { get; set; }
    public string PartyName { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public LegalAgreementStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}
