namespace BuilderERP.Application.DTOs;

public class FloorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? FloorNumber { get; set; }
    public bool IsActive { get; set; }
    public Guid TowerId { get; set; }
    public string TowerName { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateFloorDto
{
    public string Name { get; set; } = string.Empty;
    public int? FloorNumber { get; set; }
    public Guid TowerId { get; set; }
}

public class UpdateFloorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? FloorNumber { get; set; }
    public Guid TowerId { get; set; }
}
