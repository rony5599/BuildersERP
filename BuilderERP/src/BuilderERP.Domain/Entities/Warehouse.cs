namespace BuilderERP.Domain.Entities;

public class Warehouse : BaseEntity
{
    public string WarehouseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;

    public long BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
