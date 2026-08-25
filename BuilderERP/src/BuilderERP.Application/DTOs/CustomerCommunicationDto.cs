using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class CustomerCommunicationDto
{
    public long Id { get; set; }
    public DateTime CommunicationDate { get; set; }
    public CommunicationType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public long CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
}

public class CreateCustomerCommunicationDto
{
    public DateTime CommunicationDate { get; set; } = DateTime.UtcNow;
    public CommunicationType Type { get; set; } = CommunicationType.Call;
    public string Subject { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public long CustomerId { get; set; }
}

public class UpdateCustomerCommunicationDto
{
    public long Id { get; set; }
    public DateTime CommunicationDate { get; set; }
    public CommunicationType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public long CustomerId { get; set; }
}
