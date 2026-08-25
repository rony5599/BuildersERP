using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class ServiceTicketDto
{
    public long Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ServiceTicketCategory Category { get; set; }
    public RequestPriority Priority { get; set; }
    public ServiceTicketStatus Status { get; set; }
    public DateTime RaisedDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public long PropertyUnitId { get; set; }
    public bool IsActive { get; set; }
    public string PropertyUnitName { get; set; } = string.Empty;
}

public class CreateServiceTicketDto
{
    public string TicketNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ServiceTicketCategory Category { get; set; }
    public RequestPriority Priority { get; set; }
    public ServiceTicketStatus Status { get; set; }
    public DateTime RaisedDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public long PropertyUnitId { get; set; }
}

public class UpdateServiceTicketDto
{
    public long Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ServiceTicketCategory Category { get; set; }
    public RequestPriority Priority { get; set; }
    public ServiceTicketStatus Status { get; set; }
    public DateTime RaisedDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public long PropertyUnitId { get; set; }
}
