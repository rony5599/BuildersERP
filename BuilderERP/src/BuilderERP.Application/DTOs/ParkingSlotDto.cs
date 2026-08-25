using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class ParkingSlotDto
{
    public long Id { get; set; }
    public string SlotNumber { get; set; } = string.Empty;
    public ParkingSlotType SlotType { get; set; }
    public ParkingSlotStatus Status { get; set; }
    public string? AllocatedTo { get; set; }
    public string? Remarks { get; set; }
    public long ProjectId { get; set; }
    public bool IsActive { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateParkingSlotDto
{
    public string SlotNumber { get; set; } = string.Empty;
    public ParkingSlotType SlotType { get; set; }
    public ParkingSlotStatus Status { get; set; }
    public string? AllocatedTo { get; set; }
    public string? Remarks { get; set; }
    public long ProjectId { get; set; }
}

public class UpdateParkingSlotDto
{
    public long Id { get; set; }
    public string SlotNumber { get; set; } = string.Empty;
    public ParkingSlotType SlotType { get; set; }
    public ParkingSlotStatus Status { get; set; }
    public string? AllocatedTo { get; set; }
    public string? Remarks { get; set; }
    public long ProjectId { get; set; }
}
