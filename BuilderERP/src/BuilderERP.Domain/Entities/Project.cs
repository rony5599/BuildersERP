namespace BuilderERP.Domain.Entities;

public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    public long BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public ICollection<CostCenter> CostCenters { get; set; } = new List<CostCenter>();
}
