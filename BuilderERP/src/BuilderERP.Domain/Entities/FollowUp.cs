using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class FollowUp : BaseEntity
{
    public DateTime FollowUpDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public DateTime? NextFollowUpDate { get; set; }
    public FollowUpOutcome? Outcome { get; set; }
    public bool IsActive { get; set; } = true;

    public long LeadId { get; set; }
    public Lead Lead { get; set; } = null!;
}
