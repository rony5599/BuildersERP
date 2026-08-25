namespace BuilderERP.Application.DTOs;

public class StockAdjustmentDto
{
    public long Id { get; set; }
    public string AdjustmentNumber { get; set; } = string.Empty;
    public DateTime AdjustmentDate { get; set; }
    public decimal QuantityDelta { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public long WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateStockAdjustmentDto
{
    public string AdjustmentNumber { get; set; } = string.Empty;
    public DateTime AdjustmentDate { get; set; } = DateTime.UtcNow;
    public decimal QuantityDelta { get; set; }
    public string Reason { get; set; } = string.Empty;
    public long MaterialId { get; set; }
    public long WarehouseId { get; set; }
}

public class UpdateStockAdjustmentDto
{
    public long Id { get; set; }
    public string AdjustmentNumber { get; set; } = string.Empty;
    public DateTime AdjustmentDate { get; set; }
    public decimal QuantityDelta { get; set; }
    public string Reason { get; set; } = string.Empty;
    public long MaterialId { get; set; }
    public long WarehouseId { get; set; }
}
