using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class NcrDto
{
    public long Id { get; set; }
    public string NcrNumber { get; set; } = string.Empty;
    public DateTime RaisedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public NcrSeverity Severity { get; set; }
    public NcrStatus Status { get; set; }
    public string? ResolutionDescription { get; set; }
    public DateTime? ClosedDate { get; set; }
    public bool IsActive { get; set; }
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateNcrDto
{
    public string NcrNumber { get; set; } = string.Empty;
    public DateTime RaisedDate { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public NcrSeverity Severity { get; set; } = NcrSeverity.Minor;
    public NcrStatus Status { get; set; } = NcrStatus.Open;
    public string? ResolutionDescription { get; set; }
    public DateTime? ClosedDate { get; set; }
    public long ProjectId { get; set; }
}

public class UpdateNcrDto
{
    public long Id { get; set; }
    public string NcrNumber { get; set; } = string.Empty;
    public DateTime RaisedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public NcrSeverity Severity { get; set; }
    public NcrStatus Status { get; set; }
    public string? ResolutionDescription { get; set; }
    public DateTime? ClosedDate { get; set; }
    public long ProjectId { get; set; }
}
