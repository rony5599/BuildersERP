using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Drawing : BaseEntity
{
    public string DrawingNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DrawingDiscipline Discipline { get; set; } = DrawingDiscipline.Architectural;
    public string FilePath { get; set; } = string.Empty;
    public DrawingStatus Status { get; set; } = DrawingStatus.Draft;
    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
