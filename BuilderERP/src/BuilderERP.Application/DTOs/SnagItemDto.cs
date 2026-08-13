using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class SnagItemDto
{
    public Guid Id { get; set; }
    public string SnagNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public SnagSeverity Severity { get; set; }
    public SnagStatus Status { get; set; }
    public DateTime ReportedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
    public bool IsActive { get; set; }
    public string PropertyUnitName { get; set; } = string.Empty;
}

public class CreateSnagItemDto
{
    public string SnagNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public SnagSeverity Severity { get; set; }
    public SnagStatus Status { get; set; }
    public DateTime ReportedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
}

public class UpdateSnagItemDto
{
    public Guid Id { get; set; }
    public string SnagNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public SnagSeverity Severity { get; set; }
    public SnagStatus Status { get; set; }
    public DateTime ReportedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
}
