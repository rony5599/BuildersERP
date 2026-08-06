namespace BuilderERP.Application.DTOs;

public class WarehouseDto
{
    public Guid Id { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public bool IsActive { get; set; }
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
}

public class CreateWarehouseDto
{
    public string WarehouseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public Guid BranchId { get; set; }
}

public class UpdateWarehouseDto
{
    public Guid Id { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public Guid BranchId { get; set; }
}
