using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class CustomerCommunication : BaseEntity
{
    public DateTime CommunicationDate { get; set; } = DateTime.UtcNow;
    public CommunicationType Type { get; set; } = CommunicationType.Call;
    public string Subject { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}
