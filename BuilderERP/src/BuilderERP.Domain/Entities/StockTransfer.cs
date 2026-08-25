namespace BuilderERP.Domain.Entities;

public class StockTransfer : BaseEntity
{
    public string TransferNumber { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; } = DateTime.UtcNow;
    public decimal Quantity { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public long MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public long FromWarehouseId { get; set; }
    public Warehouse FromWarehouse { get; set; } = null!;

    public long ToWarehouseId { get; set; }
    public Warehouse ToWarehouse { get; set; } = null!;
}
