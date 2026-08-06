using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Booking : BaseEntity
{
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public decimal BookingAmount { get; set; }
    public BookingRequestStatus Status { get; set; } = BookingRequestStatus.Pending;
    public bool IsActive { get; set; } = true;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid PropertyUnitId { get; set; }
    public PropertyUnit PropertyUnit { get; set; } = null!;
}
