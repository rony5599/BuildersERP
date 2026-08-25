using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class EngineerWorkOrderRequisitionDetail : BaseEntity
{
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal EstimatedUnitPrice { get; set; }
    public decimal EstimatedAmount { get; set; }
    public string? Remarks { get; set; }

    public Guid EngineerWorkOrderRequisitionId { get; set; }
    public EngineerWorkOrderRequisition EngineerWorkOrderRequisition { get; set; } = null!;

    public Guid MaterialId { get; set; }
    public Material Material { get; set; } = null!;
}
