using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class PropertyUnitDto
{
    public long Id { get; set; }
    public string UnitNumber { get; set; } = string.Empty;
    public UnitType UnitType { get; set; }
    public decimal? Area { get; set; }
    public decimal? Price { get; set; }
    public BookingStatus BookingStatus { get; set; }
    public bool IsActive { get; set; }
    public long FloorId { get; set; }
    public string FloorName { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreatePropertyUnitDto
{
    public string UnitNumber { get; set; } = string.Empty;
    public UnitType UnitType { get; set; }
    public decimal? Area { get; set; }
    public decimal? Price { get; set; }
    public BookingStatus BookingStatus { get; set; } = BookingStatus.Available;
    public long FloorId { get; set; }
}

public class UpdatePropertyUnitDto
{
    public long Id { get; set; }
    public string UnitNumber { get; set; } = string.Empty;
    public UnitType UnitType { get; set; }
    public decimal? Area { get; set; }
    public decimal? Price { get; set; }
    public BookingStatus BookingStatus { get; set; }
    public long FloorId { get; set; }
}
