namespace BuilderERP.Application.DTOs;

public class FuelLogDto
{
    public Guid Id { get; set; }
    public Guid EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public DateTime LogDate { get; set; }
    public decimal FuelQuantity { get; set; }
    public decimal FuelCost { get; set; }
    public decimal? MeterReading { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
}

public class CreateFuelLogDto
{
    public Guid EquipmentId { get; set; }
    public DateTime LogDate { get; set; } = DateTime.UtcNow;
    public decimal FuelQuantity { get; set; }
    public decimal FuelCost { get; set; }
    public decimal? MeterReading { get; set; }
    public string? Remarks { get; set; }
}

public class UpdateFuelLogDto
{
    public Guid Id { get; set; }
    public Guid EquipmentId { get; set; }
    public DateTime LogDate { get; set; }
    public decimal FuelQuantity { get; set; }
    public decimal FuelCost { get; set; }
    public decimal? MeterReading { get; set; }
    public string? Remarks { get; set; }
}
