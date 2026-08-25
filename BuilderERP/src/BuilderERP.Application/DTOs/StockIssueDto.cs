namespace BuilderERP.Application.DTOs;

public class StockIssueDto
{
    public long Id { get; set; }
    public string IssueNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public decimal Quantity { get; set; }
    public string IssuedTo { get; set; } = string.Empty;
    public decimal ConsumedQuantity { get; set; }
    public decimal WastageQuantity { get; set; }
    public string? WastageReason { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public long WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateStockIssueDto
{
    public string IssueNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public decimal Quantity { get; set; }
    public string IssuedTo { get; set; } = string.Empty;
    public decimal ConsumedQuantity { get; set; }
    public decimal WastageQuantity { get; set; }
    public string? WastageReason { get; set; }
    public string? Remarks { get; set; }
    public long MaterialId { get; set; }
    public long WarehouseId { get; set; }
}

public class UpdateStockIssueDto
{
    public long Id { get; set; }
    public string IssueNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public decimal Quantity { get; set; }
    public string IssuedTo { get; set; } = string.Empty;
    public decimal ConsumedQuantity { get; set; }
    public decimal WastageQuantity { get; set; }
    public string? WastageReason { get; set; }
    public string? Remarks { get; set; }
    public long MaterialId { get; set; }
    public long WarehouseId { get; set; }
}
