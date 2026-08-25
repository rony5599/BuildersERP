namespace BuilderERP.Domain.Entities;

public class ItemCategory : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public long? ParentCategoryId { get; set; }
    public ItemCategory? ParentCategory { get; set; }
}
