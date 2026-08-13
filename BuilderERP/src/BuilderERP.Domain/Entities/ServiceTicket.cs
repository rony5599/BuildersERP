using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class ServiceTicket : BaseEntity
{
    public string TicketNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ServiceTicketCategory Category { get; set; } = ServiceTicketCategory.Complaint;
    public RequestPriority Priority { get; set; } = RequestPriority.Low;
    public ServiceTicketStatus Status { get; set; } = ServiceTicketStatus.Open;
    public DateTime RaisedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid PropertyUnitId { get; set; }
    public PropertyUnit PropertyUnit { get; set; } = null!;
}
