namespace BuilderERP.Domain.Entities;

public class StockIssue : BaseEntity
{
    public string IssueNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public decimal Quantity { get; set; }
    public string IssuedTo { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
}
