using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class RateContract : BaseEntity
{
    public string ContractNumber { get; set; } = string.Empty;
    public string ItemDescription { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal Rate { get; set; }
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;

    public long ContractorId { get; set; }
    public Contractor Contractor { get; set; } = null!;

    public long? ProjectId { get; set; }
    public Project? Project { get; set; }
}
