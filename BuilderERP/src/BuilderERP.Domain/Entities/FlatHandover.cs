using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class FlatHandover : BaseEntity
{
    public string HandoverNumber { get; set; } = string.Empty;
    public DateTime HandoverDate { get; set; } = DateTime.UtcNow;
    public string? KeyIssuedTo { get; set; }
    public HandoverStatus Status { get; set; } = HandoverStatus.Scheduled;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid PropertyUnitId { get; set; }
    public PropertyUnit PropertyUnit { get; set; } = null!;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}
