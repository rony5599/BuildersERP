using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class CommonAreaBooking : BaseEntity
{
    public string BookingNumber { get; set; } = string.Empty;
    public string FacilityName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime EndTime { get; set; } = DateTime.UtcNow;
    public decimal Fee { get; set; }
    public CommonAreaBookingStatus Status { get; set; } = CommonAreaBookingStatus.Requested;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
