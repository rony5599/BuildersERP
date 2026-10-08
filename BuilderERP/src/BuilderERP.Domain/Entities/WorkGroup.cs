namespace BuilderERP.Domain.Entities;

public class WorkGroup : BaseEntity
{
    public long? ParentGroupId { get; set; }
    public WorkGroup? ParentGroup { get; set; }
    public ICollection<WorkGroup> Children { get; set; } = new List<WorkGroup>();

    public string GroupCode { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;

    public ICollection<BoqItem> BoqItems { get; set; } = new List<BoqItem>();
}
