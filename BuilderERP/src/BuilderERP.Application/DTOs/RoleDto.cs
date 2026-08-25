namespace BuilderERP.Application.DTOs;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateRoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class PermissionGroupDto
{
    public string Module { get; set; } = string.Empty;
    public IReadOnlyList<PermissionItemDto> Permissions { get; set; } = Array.Empty<PermissionItemDto>();
}

public class PermissionItemDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
}

public class RolePermissionsDto
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public IReadOnlyList<PermissionGroupDto> Groups { get; set; } = Array.Empty<PermissionGroupDto>();
}
