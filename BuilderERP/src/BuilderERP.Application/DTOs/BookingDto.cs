using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class BookingDto
{
    public Guid Id { get; set; }
    public DateTime BookingDate { get; set; }
    public decimal BookingAmount { get; set; }
    public BookingRequestStatus Status { get; set; }
    public string? CancellationReason { get; set; }
    public bool IsActive { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid PropertyUnitId { get; set; }
    public string PropertyUnitNumber { get; set; } = string.Empty;
    public Guid? BrokerId { get; set; }
    public string? BrokerName { get; set; }
    public Guid? CollectionOfficerId { get; set; }
    public string? CollectionOfficerName { get; set; }
}

public class CreateBookingDto
{
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public decimal BookingAmount { get; set; }
    public BookingRequestStatus Status { get; set; } = BookingRequestStatus.Pending;
    public string? CancellationReason { get; set; }
    public Guid CustomerId { get; set; }
    public Guid PropertyUnitId { get; set; }
    public Guid? BrokerId { get; set; }
    public Guid? CollectionOfficerId { get; set; }
}

public class UpdateBookingDto
{
    public Guid Id { get; set; }
    public DateTime BookingDate { get; set; }
    public decimal BookingAmount { get; set; }
    public BookingRequestStatus Status { get; set; }
    public string? CancellationReason { get; set; }
    public Guid CustomerId { get; set; }
    public Guid PropertyUnitId { get; set; }
    public Guid? BrokerId { get; set; }
    public Guid? CollectionOfficerId { get; set; }
}
