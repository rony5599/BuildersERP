namespace BuilderERP.Application.DTOs;

public class WarehouseDto
{
    public long Id { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public bool IsActive { get; set; }
    public long BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateWarehouseDto
{
    public string WarehouseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public long BranchId { get; set; }
    public long ProjectId { get; set; }
}

public class UpdateWarehouseDto
{
    public long Id { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public long BranchId { get; set; }
    public long ProjectId { get; set; }
}
