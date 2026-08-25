using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class CustomerDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? NIDNumber { get; set; }
    public KycStatus KycStatus { get; set; }
    public bool IsActive { get; set; }
    public long CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public long? LeadId { get; set; }
    public string? LeadName { get; set; }
}

public class CreateCustomerDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? NIDNumber { get; set; }
    public KycStatus KycStatus { get; set; } = KycStatus.Pending;
    public long CompanyId { get; set; }
    public long? LeadId { get; set; }
}

public class UpdateCustomerDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? NIDNumber { get; set; }
    public KycStatus KycStatus { get; set; }
    public long CompanyId { get; set; }
    public long? LeadId { get; set; }
}
