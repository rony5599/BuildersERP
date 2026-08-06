namespace BuilderERP.Application.DTOs;

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
}

public class CreateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
}

public class UpdateDepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
}
