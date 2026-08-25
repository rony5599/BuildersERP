namespace BuilderERP.Domain.Entities;

public class Stock : BaseEntity
{
    public decimal QuantityOnHand { get; set; }
    public bool IsActive { get; set; } = true;

    public long MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public long WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
}
