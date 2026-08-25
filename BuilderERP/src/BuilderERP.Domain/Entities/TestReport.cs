using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class TestReport : BaseEntity
{
    public string ReportNumber { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public DateTime TestDate { get; set; } = DateTime.UtcNow;
    public string? LabName { get; set; }
    public QcResult Result { get; set; } = QcResult.Pending;
    public string FilePath { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public long? MaterialId { get; set; }
    public Material? Material { get; set; }
}
