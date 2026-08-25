namespace BuilderERP.Application.DTOs;

public class BuildingDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? TotalFloors { get; set; }
    public bool IsActive { get; set; }
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateBuildingDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? TotalFloors { get; set; }
    public long ProjectId { get; set; }
}

public class UpdateBuildingDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? TotalFloors { get; set; }
    public long ProjectId { get; set; }
}
