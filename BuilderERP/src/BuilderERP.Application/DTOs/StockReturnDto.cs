namespace BuilderERP.Application.DTOs;

public class StockReturnDto
{
    public long Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; }
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public long WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateStockReturnDto
{
    public string ReturnNumber { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public long MaterialId { get; set; }
    public long WarehouseId { get; set; }
}

public class UpdateStockReturnDto
{
    public long Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; }
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public long MaterialId { get; set; }
    public long WarehouseId { get; set; }
}
