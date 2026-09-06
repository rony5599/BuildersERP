using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class CashRequisitionDetail : BaseEntity
{
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal EstimatedUnitPrice { get; set; }
    public decimal EstimatedAmount { get; set; }
    public string? Remarks { get; set; }

    public long CashRequisitionId { get; set; }
    public CashRequisition CashRequisition { get; set; } = null!;

    public long MaterialId { get; set; }
    public Material Material { get; set; } = null!;
}
