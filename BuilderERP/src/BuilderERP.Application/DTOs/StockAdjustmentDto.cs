namespace BuilderERP.Application.DTOs;

public class StockAdjustmentDto
{
    public Guid Id { get; set; }
    public string AdjustmentNumber { get; set; } = string.Empty;
    public DateTime AdjustmentDate { get; set; }
    public decimal QuantityDelta { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
}

public class CreateStockAdjustmentDto
{
    public string AdjustmentNumber { get; set; } = string.Empty;
    public DateTime AdjustmentDate { get; set; } = DateTime.UtcNow;
    public decimal QuantityDelta { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid MaterialId { get; set; }
    public Guid WarehouseId { get; set; }
}

public class UpdateStockAdjustmentDto
{
    public Guid Id { get; set; }
    public string AdjustmentNumber { get; set; } = string.Empty;
    public DateTime AdjustmentDate { get; set; }
    public decimal QuantityDelta { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid MaterialId { get; set; }
    public Guid WarehouseId { get; set; }
}
