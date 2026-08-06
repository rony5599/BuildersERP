namespace BuilderERP.Application.DTOs;

public class StockReturnDto
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; }
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
}

public class CreateStockReturnDto
{
    public string ReturnNumber { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid MaterialId { get; set; }
    public Guid WarehouseId { get; set; }
}

public class UpdateStockReturnDto
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; }
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid MaterialId { get; set; }
    public Guid WarehouseId { get; set; }
}
