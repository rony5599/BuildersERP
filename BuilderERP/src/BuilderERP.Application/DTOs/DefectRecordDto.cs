using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class DefectRecordDto
{
    public long Id { get; set; }
    public string DefectNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DefectCategory Category { get; set; }
    public DefectSeverity Severity { get; set; }
    public DefectStatus Status { get; set; }
    public DateTime ReportedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public long PropertyUnitId { get; set; }
    public bool IsActive { get; set; }
    public string PropertyUnitName { get; set; } = string.Empty;
}

public class CreateDefectRecordDto
{
    public string DefectNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DefectCategory Category { get; set; }
    public DefectSeverity Severity { get; set; }
    public DefectStatus Status { get; set; }
    public DateTime ReportedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public long PropertyUnitId { get; set; }
}

public class UpdateDefectRecordDto
{
    public long Id { get; set; }
    public string DefectNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DefectCategory Category { get; set; }
    public DefectSeverity Severity { get; set; }
    public DefectStatus Status { get; set; }
    public DateTime ReportedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public long PropertyUnitId { get; set; }
}
