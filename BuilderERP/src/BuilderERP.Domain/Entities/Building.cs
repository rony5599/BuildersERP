namespace BuilderERP.Domain.Entities;

public class Building : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? TotalFloors { get; set; }
    public bool IsActive { get; set; } = true;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ICollection<Tower> Towers { get; set; } = new List<Tower>();
}
