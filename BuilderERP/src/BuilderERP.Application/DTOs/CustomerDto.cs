using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? NIDNumber { get; set; }
    public KycStatus KycStatus { get; set; }
    public bool IsActive { get; set; }
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public Guid? LeadId { get; set; }
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
    public Guid CompanyId { get; set; }
    public Guid? LeadId { get; set; }
}

public class UpdateCustomerDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? NIDNumber { get; set; }
    public KycStatus KycStatus { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? LeadId { get; set; }
}
