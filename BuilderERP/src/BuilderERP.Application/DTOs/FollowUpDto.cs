using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class FollowUpDto
{
    public long Id { get; set; }
    public DateTime FollowUpDate { get; set; }
    public string? Notes { get; set; }
    public DateTime? NextFollowUpDate { get; set; }
    public FollowUpOutcome? Outcome { get; set; }
    public bool IsActive { get; set; }
    public long LeadId { get; set; }
    public string LeadName { get; set; } = string.Empty;
}

public class CreateFollowUpDto
{
    public DateTime FollowUpDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public DateTime? NextFollowUpDate { get; set; }
    public FollowUpOutcome? Outcome { get; set; }
    public long LeadId { get; set; }
}

public class UpdateFollowUpDto
{
    public long Id { get; set; }
    public DateTime FollowUpDate { get; set; }
    public string? Notes { get; set; }
    public DateTime? NextFollowUpDate { get; set; }
    public FollowUpOutcome? Outcome { get; set; }
    public long LeadId { get; set; }
}
