namespace BuilderERP.Application.DTOs;

public class StockTransferDto
{
    public long Id { get; set; }
    public string TransferNumber { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
    public decimal Quantity { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public long FromWarehouseId { get; set; }
    public string FromWarehouseName { get; set; } = string.Empty;
    public long ToWarehouseId { get; set; }
    public string ToWarehouseName { get; set; } = string.Empty;
    public long FromProjectId { get; set; }
    public string FromProjectName { get; set; } = string.Empty;
    public long ToProjectId { get; set; }
    public string ToProjectName { get; set; } = string.Empty;
}

public class CreateStockTransferDto
{
    public string TransferNumber { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; } = DateTime.UtcNow;
    public decimal Quantity { get; set; }
    public string? Remarks { get; set; }
    public long MaterialId { get; set; }
    public long FromWarehouseId { get; set; }
    public long ToWarehouseId { get; set; }
}

public class UpdateStockTransferDto
{
    public long Id { get; set; }
    public string TransferNumber { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
    public decimal Quantity { get; set; }
    public string? Remarks { get; set; }
    public long MaterialId { get; set; }
    public long FromWarehouseId { get; set; }
    public long ToWarehouseId { get; set; }
}
