namespace BuilderERP.Domain.Entities;

public class StockReturn : BaseEntity
{
    public string ReturnNumber { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Guid MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
}
