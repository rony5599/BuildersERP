using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class GoodsReceiveDetail : BaseEntity
{
    public decimal ReceivedQuantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal UnitPrice { get; set; }
    public string? BatchNo { get; set; }
    public string? SerialNo { get; set; }
    public decimal VatPercent { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }

    public Guid GoodsReceiveId { get; set; }
    public GoodsReceive GoodsReceive { get; set; } = null!;

    public Guid PurchaseOrderDetailId { get; set; }
    public PurchaseOrderDetail PurchaseOrderDetail { get; set; } = null!;

    public Guid MaterialId { get; set; }
    public Material Material { get; set; } = null!;
}
