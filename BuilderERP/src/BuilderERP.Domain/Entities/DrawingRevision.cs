namespace BuilderERP.Domain.Entities;

public class DrawingRevision : BaseEntity
{
    public string RevisionCode { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime RevisedDate { get; set; } = DateTime.UtcNow;
    public string? ChangeDescription { get; set; }
    public bool IsCurrent { get; set; } = true;
    public bool IsActive { get; set; } = true;

    public long DrawingId { get; set; }
    public Drawing Drawing { get; set; } = null!;
}
