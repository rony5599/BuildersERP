using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class SiteVisitDto
{
    public long Id { get; set; }
    public DateTime VisitDate { get; set; }
    public SiteVisitStatus Status { get; set; }
    public string? Feedback { get; set; }
    public bool IsActive { get; set; }
    public long LeadId { get; set; }
    public string LeadName { get; set; } = string.Empty;
    public long? PropertyUnitId { get; set; }
    public string? PropertyUnitNumber { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
}

public class CreateSiteVisitDto
{
    public DateTime VisitDate { get; set; } = DateTime.UtcNow;
    public SiteVisitStatus Status { get; set; } = SiteVisitStatus.Scheduled;
    public string? Feedback { get; set; }
    public long LeadId { get; set; }
    public long? PropertyUnitId { get; set; }
    public Guid? AssignedToUserId { get; set; }
}

public class UpdateSiteVisitDto
{
    public long Id { get; set; }
    public DateTime VisitDate { get; set; }
    public SiteVisitStatus Status { get; set; }
    public string? Feedback { get; set; }
    public long LeadId { get; set; }
    public long? PropertyUnitId { get; set; }
    public Guid? AssignedToUserId { get; set; }
}
