using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class PurchaseOrder : BaseEntity
{
    public string PONumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public decimal ReceivedAmount { get; set; }
    public DateTime DeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public bool IsActive { get; set; } = true;

    public long VendorQuotationId { get; set; }
    public VendorQuotation VendorQuotation { get; set; } = null!;

    public ICollection<PurchaseOrderDetail> Details { get; set; } = new List<PurchaseOrderDetail>();
    public ICollection<GoodsReceive> GoodsReceives { get; set; } = new List<GoodsReceive>();
}
