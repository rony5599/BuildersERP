using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class DrawingDto
{
    public Guid Id { get; set; }
    public string DrawingNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DrawingDiscipline Discipline { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DrawingStatus Status { get; set; }
    public DateTime UploadedDate { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateDrawingDto
{
    public string DrawingNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DrawingDiscipline Discipline { get; set; } = DrawingDiscipline.Architectural;
    public string FilePath { get; set; } = string.Empty;
    public DrawingStatus Status { get; set; } = DrawingStatus.Draft;
    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateDrawingDto
{
    public Guid Id { get; set; }
    public string DrawingNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DrawingDiscipline Discipline { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DrawingStatus Status { get; set; }
    public DateTime UploadedDate { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}
