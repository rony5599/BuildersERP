using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class PurchaseRequisitionDetail : BaseEntity
{
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal EstimatedUnitPrice { get; set; }
    public decimal EstimatedAmount { get; set; }
    public string? Remarks { get; set; }

    public long PurchaseRequisitionId { get; set; }
    public PurchaseRequisition PurchaseRequisition { get; set; } = null!;

    public long MaterialId { get; set; }
    public Material Material { get; set; } = null!;
}
