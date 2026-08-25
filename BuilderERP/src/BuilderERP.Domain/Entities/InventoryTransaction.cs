using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class InventoryTransaction : BaseEntity
{
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public InventoryTransactionType TransactionType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string? BatchNo { get; set; }
    public string? SerialNo { get; set; }
    public decimal QuantityIn { get; set; }
    public decimal QuantityOut { get; set; }
    public decimal BalanceQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }

    public long MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public long WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
}
