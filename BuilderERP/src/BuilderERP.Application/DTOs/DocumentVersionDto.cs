namespace BuilderERP.Application.DTOs;

public class DocumentVersionDto
{
    public long Id { get; set; }
    public string VersionNumber { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedDate { get; set; }
    public string? ChangeNotes { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsActive { get; set; }
    public long DocumentId { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
}

public class CreateDocumentVersionDto
{
    public string VersionNumber { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    public string? ChangeNotes { get; set; }
    public bool IsCurrent { get; set; } = true;
    public long DocumentId { get; set; }
}

public class UpdateDocumentVersionDto
{
    public long Id { get; set; }
    public string VersionNumber { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedDate { get; set; }
    public string? ChangeNotes { get; set; }
    public bool IsCurrent { get; set; }
    public long DocumentId { get; set; }
}
