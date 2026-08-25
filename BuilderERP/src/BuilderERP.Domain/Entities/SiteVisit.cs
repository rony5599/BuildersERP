using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class SiteVisit : BaseEntity
{
    public DateTime VisitDate { get; set; } = DateTime.UtcNow;
    public SiteVisitStatus Status { get; set; } = SiteVisitStatus.Scheduled;
    public string? Feedback { get; set; }
    public bool IsActive { get; set; } = true;

    public long LeadId { get; set; }
    public Lead Lead { get; set; } = null!;

    public long? PropertyUnitId { get; set; }
    public PropertyUnit? PropertyUnit { get; set; }

    public Guid? AssignedToUserId { get; set; }
    public ApplicationUser? AssignedToUser { get; set; }
}
