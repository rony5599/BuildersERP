using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class SafetyAuditDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime AuditDate { get; set; }
    public string AuditedBy { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string? Findings { get; set; }
    public SafetyAuditStatus Status { get; set; }
    public bool IsActive { get; set; }
}

public class CreateSafetyAuditDto
{
    public long ProjectId { get; set; }
    public DateTime AuditDate { get; set; } = DateTime.UtcNow;
    public string AuditedBy { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string? Findings { get; set; }
    public SafetyAuditStatus Status { get; set; } = SafetyAuditStatus.Scheduled;
}

public class UpdateSafetyAuditDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public DateTime AuditDate { get; set; }
    public string AuditedBy { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string? Findings { get; set; }
    public SafetyAuditStatus Status { get; set; }
}
