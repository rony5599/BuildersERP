using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class CashPurchaseOrderDetail : BaseEntity
{
    public decimal OrderedQuantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal VatPercent { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public decimal ReceivedQuantity { get; set; }

    public long CashPurchaseOrderId { get; set; }
    public CashPurchaseOrder CashPurchaseOrder { get; set; } = null!;

    public long MaterialId { get; set; }
    public Material Material { get; set; } = null!;
}
