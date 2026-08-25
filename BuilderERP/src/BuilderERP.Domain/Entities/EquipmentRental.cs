using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class EquipmentRental : BaseEntity
{
    public long EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;

    public long? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public DateTime RentalStartDate { get; set; } = DateTime.UtcNow;
    public DateTime? RentalEndDate { get; set; }
    public decimal RatePerDay { get; set; }
    public decimal TotalAmount { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.Active;
    public bool IsActive { get; set; } = true;
}
