namespace BuilderERP.Application.DTOs;

public class FloorDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? FloorNumber { get; set; }
    public bool IsActive { get; set; }
    public long TowerId { get; set; }
    public string TowerName { get; set; } = string.Empty;
    public long BuildingId { get; set; }
    public string BuildingName { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateFloorDto
{
    public string Name { get; set; } = string.Empty;
    public int? FloorNumber { get; set; }
    public long TowerId { get; set; }
}

public class UpdateFloorDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? FloorNumber { get; set; }
    public long TowerId { get; set; }
}
