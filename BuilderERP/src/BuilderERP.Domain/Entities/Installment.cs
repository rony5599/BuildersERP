using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Installment : BaseEntity
{
    public int InstallmentNumber { get; set; }
    public DateTime DueDate { get; set; }
    public decimal DueAmount { get; set; }
    public decimal PenaltyAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public InstallmentStatus Status { get; set; } = InstallmentStatus.Pending;
    public bool IsRescheduled { get; set; }
    public DateTime? OriginalDueDate { get; set; }
    public string? RescheduleReason { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid InstallmentPlanId { get; set; }
    public InstallmentPlan InstallmentPlan { get; set; } = null!;
}
