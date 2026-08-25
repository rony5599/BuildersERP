using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class SiteInspection : BaseEntity
{
    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public DateTime InspectionDate { get; set; } = DateTime.UtcNow;
    public string InspectedBy { get; set; } = string.Empty;
    public string? Location { get; set; }
    public QcResult Result { get; set; } = QcResult.Pending;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;
}
