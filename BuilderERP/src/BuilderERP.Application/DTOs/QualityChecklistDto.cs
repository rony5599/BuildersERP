using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class QualityChecklistDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ChecklistName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public DateTime ChecklistDate { get; set; }
    public string CheckedBy { get; set; } = string.Empty;
    public QcResult Result { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
}

public class CreateQualityChecklistDto
{
    public long ProjectId { get; set; }
    public string ChecklistName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public DateTime ChecklistDate { get; set; } = DateTime.UtcNow;
    public string CheckedBy { get; set; } = string.Empty;
    public QcResult Result { get; set; } = QcResult.Pending;
    public string? Remarks { get; set; }
}

public class UpdateQualityChecklistDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string ChecklistName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public DateTime ChecklistDate { get; set; }
    public string CheckedBy { get; set; } = string.Empty;
    public QcResult Result { get; set; }
    public string? Remarks { get; set; }
}
