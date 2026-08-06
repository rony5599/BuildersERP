namespace BuilderERP.Application.DTOs;

public class StockIssueDto
{
    public Guid Id { get; set; }
    public string IssueNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public decimal Quantity { get; set; }
    public string IssuedTo { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
}

public class CreateStockIssueDto
{
    public string IssueNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public decimal Quantity { get; set; }
    public string IssuedTo { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public Guid MaterialId { get; set; }
    public Guid WarehouseId { get; set; }
}

public class UpdateStockIssueDto
{
    public Guid Id { get; set; }
    public string IssueNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public decimal Quantity { get; set; }
    public string IssuedTo { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public Guid MaterialId { get; set; }
    public Guid WarehouseId { get; set; }
}
