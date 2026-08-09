namespace BuilderERP.Domain.Entities;

public class Contractor : BaseEntity
{
    public string ContractorCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Specialization { get; set; }
    public bool IsActive { get; set; } = true;
}
