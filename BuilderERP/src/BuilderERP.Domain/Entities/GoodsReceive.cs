using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class GoodsReceive : BaseEntity
{
    public string GrnNumber { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;
    public decimal ReceivedAmount { get; set; }
    public string? Remarks { get; set; }
    public GrnStatus Status { get; set; } = GrnStatus.Draft;
    public bool IsActive { get; set; } = true;

    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public ICollection<GoodsReceiveDetail> Details { get; set; } = new List<GoodsReceiveDetail>();
}
