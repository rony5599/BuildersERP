using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class DefectRecord : BaseEntity
{
    public string DefectNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DefectCategory Category { get; set; } = DefectCategory.Structural;
    public DefectSeverity Severity { get; set; } = DefectSeverity.Low;
    public DefectStatus Status { get; set; } = DefectStatus.Reported;
    public DateTime ReportedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public long PropertyUnitId { get; set; }
    public PropertyUnit PropertyUnit { get; set; } = null!;
}
