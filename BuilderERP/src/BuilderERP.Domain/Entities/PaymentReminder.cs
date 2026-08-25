using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class PaymentReminder : BaseEntity
{
    public ReminderChannel Channel { get; set; } = ReminderChannel.SMS;
    public DateTime ScheduledDate { get; set; } = DateTime.UtcNow;
    public string Message { get; set; } = string.Empty;
    public ReminderStatus Status { get; set; } = ReminderStatus.Pending;
    public DateTime? SentDate { get; set; }
    public bool IsActive { get; set; } = true;

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public long? InstallmentId { get; set; }
    public Installment? Installment { get; set; }
}
