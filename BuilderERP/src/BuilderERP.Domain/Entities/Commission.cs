using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Commission : BaseEntity
{
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public CommissionStatus Status { get; set; } = CommissionStatus.Pending;
    public DateTime? PaymentDate { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid BrokerId { get; set; }
    public Broker Broker { get; set; } = null!;

    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
}
