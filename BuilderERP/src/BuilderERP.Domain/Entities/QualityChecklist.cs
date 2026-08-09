using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class QualityChecklist : BaseEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public string ChecklistName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public DateTime ChecklistDate { get; set; } = DateTime.UtcNow;
    public string CheckedBy { get; set; } = string.Empty;
    public QcResult Result { get; set; } = QcResult.Pending;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;
}
