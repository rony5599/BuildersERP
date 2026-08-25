using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class InstallmentDto
{
    public long Id { get; set; }
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
    public long InstallmentPlanId { get; set; }
    public string AgreementNumber { get; set; } = string.Empty;
    public long? ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
}

public class CreateInstallmentDto
{
    public int InstallmentNumber { get; set; }
    public DateTime DueDate { get; set; } = DateTime.UtcNow;
    public decimal DueAmount { get; set; }
    public decimal PenaltyAmount { get; set; } = 0;
    public InstallmentStatus Status { get; set; } = InstallmentStatus.Pending;
    public long InstallmentPlanId { get; set; }
}

public class UpdateInstallmentDto
{
    public long Id { get; set; }
    public int InstallmentNumber { get; set; }
    public DateTime DueDate { get; set; }
    public decimal DueAmount { get; set; }
    public decimal PenaltyAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public InstallmentStatus Status { get; set; }
    public long InstallmentPlanId { get; set; }
}
