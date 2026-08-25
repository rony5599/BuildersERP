using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class PaymentReminderDto
{
    public long Id { get; set; }
    public ReminderChannel Channel { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Message { get; set; } = string.Empty;
    public ReminderStatus Status { get; set; }
    public DateTime? SentDate { get; set; }
    public bool IsActive { get; set; }
    public long CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public long? InstallmentId { get; set; }
    public int? InstallmentNumber { get; set; }
    public decimal? OutstandingAmount { get; set; }
    public DateTime? DueDate { get; set; }
}
