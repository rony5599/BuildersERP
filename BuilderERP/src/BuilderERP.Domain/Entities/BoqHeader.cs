namespace BuilderERP.Domain.Entities;

public class BoqHeader : BaseEntity
{
    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string BoqName { get; set; } = string.Empty;
    public int VersionNumber { get; set; } = 1;
    public decimal ContingencyPercent { get; set; } = 2m;
    public bool IsActive { get; set; } = true;
    public ICollection<BoqItem> Items { get; set; } = new List<BoqItem>();
}
