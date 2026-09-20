using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class PoBillDetail : BaseEntity
{
    public decimal BilledQuantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal VatPercent { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }

    public long PoBillId { get; set; }
    public PoBill PoBill { get; set; } = null!;

    public long PurchaseOrderDetailId { get; set; }
    public PurchaseOrderDetail PurchaseOrderDetail { get; set; } = null!;

    public long MaterialId { get; set; }
    public Material Material { get; set; } = null!;
}
