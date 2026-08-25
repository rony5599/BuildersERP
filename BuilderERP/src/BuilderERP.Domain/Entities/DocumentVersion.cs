namespace BuilderERP.Domain.Entities;

public class DocumentVersion : BaseEntity
{
    public string VersionNumber { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    public string? ChangeNotes { get; set; }
    public bool IsCurrent { get; set; } = true;
    public bool IsActive { get; set; } = true;

    public long DocumentId { get; set; }
    public Document Document { get; set; } = null!;
}
