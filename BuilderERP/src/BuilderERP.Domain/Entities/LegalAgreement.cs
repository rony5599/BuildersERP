using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class LegalAgreement : BaseEntity
{
    public string AgreementNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public LegalAgreementType AgreementType { get; set; } = LegalAgreementType.JointVenture;
    public string PartyName { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public LegalAgreementStatus Status { get; set; } = LegalAgreementStatus.Draft;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
