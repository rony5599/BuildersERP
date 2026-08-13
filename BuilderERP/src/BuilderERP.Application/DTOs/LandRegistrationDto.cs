using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class LandRegistrationDto
{
    public Guid Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string DeedNumber { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public string? SubRegistryOffice { get; set; }
    public decimal RegistrationFee { get; set; }
    public LandRegistrationStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsActive { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateLandRegistrationDto
{
    public string RegistrationNumber { get; set; } = string.Empty;
    public string DeedNumber { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public string? SubRegistryOffice { get; set; }
    public decimal RegistrationFee { get; set; }
    public LandRegistrationStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateLandRegistrationDto
{
    public Guid Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string DeedNumber { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public string? SubRegistryOffice { get; set; }
    public decimal RegistrationFee { get; set; }
    public LandRegistrationStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}
