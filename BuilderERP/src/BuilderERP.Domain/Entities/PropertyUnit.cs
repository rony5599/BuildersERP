using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class PropertyUnit : BaseEntity
{
    public string UnitNumber { get; set; } = string.Empty;
    public UnitType UnitType { get; set; }
    public decimal? Area { get; set; }
    public decimal? Price { get; set; }
    public BookingStatus BookingStatus { get; set; } = BookingStatus.Available;
    public bool IsActive { get; set; } = true;

    public Guid FloorId { get; set; }
    public Floor Floor { get; set; } = null!;
}
