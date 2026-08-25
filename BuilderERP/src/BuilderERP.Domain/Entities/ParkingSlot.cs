using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class ParkingSlot : BaseEntity
{
    public string SlotNumber { get; set; } = string.Empty;
    public ParkingSlotType SlotType { get; set; } = ParkingSlotType.Car;
    public ParkingSlotStatus Status { get; set; } = ParkingSlotStatus.Vacant;
    public string? AllocatedTo { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
