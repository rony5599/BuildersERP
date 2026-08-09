using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class RateContractDto
{
    public Guid Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string ItemDescription { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal Rate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public Guid ContractorId { get; set; }
    public string ContractorName { get; set; } = string.Empty;
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
}

public class CreateRateContractDto
{
    public string ContractNumber { get; set; } = string.Empty;
    public string ItemDescription { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal Rate { get; set; }
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public Guid ContractorId { get; set; }
    public Guid? ProjectId { get; set; }
}

public class UpdateRateContractDto
{
    public Guid Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string ItemDescription { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal Rate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public Guid ContractorId { get; set; }
    public Guid? ProjectId { get; set; }
}
