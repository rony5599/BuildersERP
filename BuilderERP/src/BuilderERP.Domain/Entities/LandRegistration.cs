using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class LandRegistration : BaseEntity
{
    public string RegistrationNumber { get; set; } = string.Empty;
    public string DeedNumber { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public string? SubRegistryOffice { get; set; }
    public decimal RegistrationFee { get; set; }
    public LandRegistrationStatus Status { get; set; } = LandRegistrationStatus.Pending;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
