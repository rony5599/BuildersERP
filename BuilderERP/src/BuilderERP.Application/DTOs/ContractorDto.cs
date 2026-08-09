namespace BuilderERP.Application.DTOs;

public class ContractorDto
{
    public Guid Id { get; set; }
    public string ContractorCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Specialization { get; set; }
    public bool IsActive { get; set; }
}

public class CreateContractorDto
{
    public string ContractorCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Specialization { get; set; }
}

public class UpdateContractorDto
{
    public Guid Id { get; set; }
    public string ContractorCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Specialization { get; set; }
}
