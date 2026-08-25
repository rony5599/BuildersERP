using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Customer : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? NIDNumber { get; set; }
    public KycStatus KycStatus { get; set; } = KycStatus.Pending;
    public bool IsActive { get; set; } = true;

    public long CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public long? LeadId { get; set; }
    public Lead? Lead { get; set; }
}
