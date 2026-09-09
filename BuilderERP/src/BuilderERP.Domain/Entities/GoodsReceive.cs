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

    public GrnSourceType SourceType { get; set; } = GrnSourceType.PurchaseOrder;

    public long? PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }

    public long? EngineerWorkOrderId { get; set; }
    public EngineerWorkOrder? EngineerWorkOrder { get; set; }

    public long? CashPurchaseOrderId { get; set; }
    public CashPurchaseOrder? CashPurchaseOrder { get; set; }

    public long WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public ICollection<GoodsReceiveDetail> Details { get; set; } = new List<GoodsReceiveDetail>();
}
