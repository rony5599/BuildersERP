using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class LeadDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Source { get; set; }
    public LeadStatus Status { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
}

public class CreateLeadDto
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Source { get; set; }
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public string? Notes { get; set; }
    public Guid? AssignedToUserId { get; set; }
}

public class UpdateLeadDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Source { get; set; }
    public LeadStatus Status { get; set; }
    public string? Notes { get; set; }
    public Guid? AssignedToUserId { get; set; }
}
