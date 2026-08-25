namespace BuilderERP.Application.DTOs;

public class DrawingRevisionDto
{
    public long Id { get; set; }
    public string RevisionCode { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime RevisedDate { get; set; }
    public string? ChangeDescription { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsActive { get; set; }
    public long DrawingId { get; set; }
    public string DrawingNumber { get; set; } = string.Empty;
}

public class CreateDrawingRevisionDto
{
    public string RevisionCode { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime RevisedDate { get; set; } = DateTime.UtcNow;
    public string? ChangeDescription { get; set; }
    public bool IsCurrent { get; set; } = true;
    public long DrawingId { get; set; }
}

public class UpdateDrawingRevisionDto
{
    public long Id { get; set; }
    public string RevisionCode { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime RevisedDate { get; set; }
    public string? ChangeDescription { get; set; }
    public bool IsCurrent { get; set; }
    public long DrawingId { get; set; }
}
