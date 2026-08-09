namespace BuilderERP.Domain.Entities;

public class FuelLog : BaseEntity
{
    public Guid EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;

    public DateTime LogDate { get; set; } = DateTime.UtcNow;
    public decimal FuelQuantity { get; set; }
    public decimal FuelCost { get; set; }
    public decimal? MeterReading { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;
}
