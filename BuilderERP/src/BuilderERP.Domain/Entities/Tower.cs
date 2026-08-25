namespace BuilderERP.Domain.Entities;

public class Tower : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public long BuildingId { get; set; }
    public Building Building { get; set; } = null!;

    public ICollection<Floor> Floors { get; set; } = new List<Floor>();
}
