namespace BuilderERP.Domain.Entities;

public class Floor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int? FloorNumber { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid TowerId { get; set; }
    public Tower Tower { get; set; } = null!;

    public ICollection<PropertyUnit> Units { get; set; } = new List<PropertyUnit>();
}
