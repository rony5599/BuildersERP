using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class InstallmentDto
{
    public Guid Id { get; set; }
    public int InstallmentNumber { get; set; }
    public DateTime DueDate { get; set; }
    public decimal DueAmount { get; set; }
    public decimal PenaltyAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public InstallmentStatus Status { get; set; }
    public bool IsRescheduled { get; set; }
    public DateTime? OriginalDueDate { get; set; }
    public string? RescheduleReason { get; set; }
    public bool IsActive { get; set; }
    public Guid InstallmentPlanId { get; set; }
}

public class CreateInstallmentDto
{
    public int InstallmentNumber { get; set; }
    public DateTime DueDate { get; set; } = DateTime.UtcNow;
    public decimal DueAmount { get; set; }
    public decimal PenaltyAmount { get; set; } = 0;
    public InstallmentStatus Status { get; set; } = InstallmentStatus.Pending;
    public Guid InstallmentPlanId { get; set; }
}

public class UpdateInstallmentDto
{
    public Guid Id { get; set; }
    public int InstallmentNumber { get; set; }
    public DateTime DueDate { get; set; }
    public decimal DueAmount { get; set; }
    public decimal PenaltyAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public InstallmentStatus Status { get; set; }
    public Guid InstallmentPlanId { get; set; }
}
