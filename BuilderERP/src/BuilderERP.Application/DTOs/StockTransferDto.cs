namespace BuilderERP.Application.DTOs;

public class StockTransferDto
{
    public Guid Id { get; set; }
    public string TransferNumber { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
    public decimal Quantity { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public Guid FromWarehouseId { get; set; }
    public string FromWarehouseName { get; set; } = string.Empty;
    public Guid ToWarehouseId { get; set; }
    public string ToWarehouseName { get; set; } = string.Empty;
    public Guid FromProjectId { get; set; }
    public string FromProjectName { get; set; } = string.Empty;
    public Guid ToProjectId { get; set; }
    public string ToProjectName { get; set; } = string.Empty;
}

public class CreateStockTransferDto
{
    public string TransferNumber { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; } = DateTime.UtcNow;
    public decimal Quantity { get; set; }
    public string? Remarks { get; set; }
    public Guid MaterialId { get; set; }
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
}

public class UpdateStockTransferDto
{
    public Guid Id { get; set; }
    public string TransferNumber { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
    public decimal Quantity { get; set; }
    public string? Remarks { get; set; }
    public Guid MaterialId { get; set; }
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
}
