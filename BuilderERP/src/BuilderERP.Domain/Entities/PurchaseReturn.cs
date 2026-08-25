using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class PurchaseReturn : BaseEntity
{
    public string ReturnNumber { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
    public decimal ReturnAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public PurchaseReturnStatus Status { get; set; } = PurchaseReturnStatus.Pending;
    public bool IsActive { get; set; } = true;

    public long GoodsReceiveId { get; set; }
    public GoodsReceive GoodsReceive { get; set; } = null!;

    public ICollection<PurchaseReturnDetail> Details { get; set; } = new List<PurchaseReturnDetail>();
}
