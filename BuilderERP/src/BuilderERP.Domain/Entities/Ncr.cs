using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Ncr : BaseEntity
{
    public string NcrNumber { get; set; } = string.Empty;
    public DateTime RaisedDate { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public NcrSeverity Severity { get; set; } = NcrSeverity.Minor;
    public NcrStatus Status { get; set; } = NcrStatus.Open;
    public string? ResolutionDescription { get; set; }
    public DateTime? ClosedDate { get; set; }
    public bool IsActive { get; set; } = true;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
