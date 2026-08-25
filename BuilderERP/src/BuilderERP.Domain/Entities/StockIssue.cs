namespace BuilderERP.Domain.Entities;

public class StockIssue : BaseEntity
{
    public string IssueNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public decimal Quantity { get; set; }
    public string IssuedTo { get; set; } = string.Empty;
    public decimal ConsumedQuantity { get; set; }
    public decimal WastageQuantity { get; set; }
    public string? WastageReason { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public long MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public long WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
}
