namespace BuilderERP.Application.DTOs;

public class StockDto
{
    public long Id { get; set; }
    public decimal QuantityOnHand { get; set; }
    public bool IsActive { get; set; }
    public long MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public decimal ReorderLevel { get; set; }
    public long WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public bool IsBelowReorderLevel { get; set; }
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}
