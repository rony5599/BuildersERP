namespace BuilderERP.Domain.Entities;

public class BudgetLine : BaseEntity
{
    public string Category { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; } = DateTime.UtcNow;
    public decimal BudgetedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
