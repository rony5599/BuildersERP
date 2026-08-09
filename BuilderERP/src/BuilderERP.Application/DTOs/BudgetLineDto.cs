namespace BuilderERP.Application.DTOs;

public class BudgetLineDto
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public decimal BudgetedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateBudgetLineDto
{
    public string Category { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; } = DateTime.UtcNow;
    public decimal BudgetedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateBudgetLineDto
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public decimal BudgetedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}
