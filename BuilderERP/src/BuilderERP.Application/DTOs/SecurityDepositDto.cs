using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class SecurityDepositDto
{
    public Guid Id { get; set; }
    public Guid ContractorId { get; set; }
    public string ContractorName { get; set; } = string.Empty;
    public Guid? WorkOrderId { get; set; }
    public string? WorkOrderNumber { get; set; }
    public decimal DepositAmount { get; set; }
    public DateTime DepositDate { get; set; }
    public DateTime? RefundDate { get; set; }
    public SecurityDepositStatus Status { get; set; }
    public bool IsActive { get; set; }
}

public class CreateSecurityDepositDto
{
    public Guid ContractorId { get; set; }
    public Guid? WorkOrderId { get; set; }
    public decimal DepositAmount { get; set; }
    public DateTime DepositDate { get; set; } = DateTime.UtcNow;
    public DateTime? RefundDate { get; set; }
    public SecurityDepositStatus Status { get; set; } = SecurityDepositStatus.Held;
}

public class UpdateSecurityDepositDto
{
    public Guid Id { get; set; }
    public Guid ContractorId { get; set; }
    public Guid? WorkOrderId { get; set; }
    public decimal DepositAmount { get; set; }
    public DateTime DepositDate { get; set; }
    public DateTime? RefundDate { get; set; }
    public SecurityDepositStatus Status { get; set; }
}
