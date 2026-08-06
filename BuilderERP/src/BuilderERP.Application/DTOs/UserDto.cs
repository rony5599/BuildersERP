namespace BuilderERP.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? CompanyName { get; set; }
    public string? BranchName { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
}

public class CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid? CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class UpdateUserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Guid? CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
