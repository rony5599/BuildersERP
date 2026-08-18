namespace BuilderERP.Domain.Entities;

public class Broker : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LicenseNumber { get; set; }
    public decimal DefaultCommissionRate { get; set; }
    public bool IsActive { get; set; } = true;
}
