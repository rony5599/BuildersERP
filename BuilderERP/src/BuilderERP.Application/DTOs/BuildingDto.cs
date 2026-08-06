namespace BuilderERP.Application.DTOs;

public class BuildingDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? TotalFloors { get; set; }
    public bool IsActive { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateBuildingDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? TotalFloors { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateBuildingDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? TotalFloors { get; set; }
    public Guid ProjectId { get; set; }
}
