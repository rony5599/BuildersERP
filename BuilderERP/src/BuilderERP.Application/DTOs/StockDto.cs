namespace BuilderERP.Application.DTOs;

public class StockDto
{
    public Guid Id { get; set; }
    public decimal QuantityOnHand { get; set; }
    public bool IsActive { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public decimal ReorderLevel { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public bool IsBelowReorderLevel { get; set; }
}
