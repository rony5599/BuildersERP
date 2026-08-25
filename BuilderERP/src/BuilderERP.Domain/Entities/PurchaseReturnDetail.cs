using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class PurchaseReturnDetail : BaseEntity
{
    public decimal ReturnQuantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal UnitPrice { get; set; }
    public decimal VatPercent { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }

    public long PurchaseReturnId { get; set; }
    public PurchaseReturn PurchaseReturn { get; set; } = null!;

    public long GoodsReceiveDetailId { get; set; }
    public GoodsReceiveDetail GoodsReceiveDetail { get; set; } = null!;

    public long MaterialId { get; set; }
    public Material Material { get; set; } = null!;
}
