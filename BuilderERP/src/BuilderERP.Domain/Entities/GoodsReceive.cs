namespace BuilderERP.Domain.Entities;

public class GoodsReceive : BaseEntity
{
    public string GrnNumber { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;
    public decimal ReceivedAmount { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
}
