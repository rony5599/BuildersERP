using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class SafetyAudit : BaseEntity
{
    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public DateTime AuditDate { get; set; } = DateTime.UtcNow;
    public string AuditedBy { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string? Findings { get; set; }
    public SafetyAuditStatus Status { get; set; } = SafetyAuditStatus.Scheduled;
    public bool IsActive { get; set; } = true;
}
