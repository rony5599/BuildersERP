namespace BuilderERP.Application.DTOs;

public class ItemCategoryDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public long? ParentCategoryId { get; set; }
    public string ParentCategoryName { get; set; } = string.Empty;
}

public class CreateItemCategoryDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long? ParentCategoryId { get; set; }
}

public class UpdateItemCategoryDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long? ParentCategoryId { get; set; }
}
