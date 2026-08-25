using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class SnagItem : BaseEntity
{
    public string SnagNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public SnagSeverity Severity { get; set; } = SnagSeverity.Minor;
    public SnagStatus Status { get; set; } = SnagStatus.Open;
    public DateTime ReportedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedDate { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public long PropertyUnitId { get; set; }
    public PropertyUnit PropertyUnit { get; set; } = null!;
}
