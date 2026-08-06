namespace BuilderERP.Domain.Entities;

public class Branch : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public ICollection<Department> Departments { get; set; } = new List<Department>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
