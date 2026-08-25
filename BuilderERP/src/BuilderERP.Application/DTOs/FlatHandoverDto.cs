using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class FlatHandoverDto
{
    public long Id { get; set; }
    public string HandoverNumber { get; set; } = string.Empty;
    public DateTime HandoverDate { get; set; }
    public string? KeyIssuedTo { get; set; }
    public HandoverStatus Status { get; set; }
    public string? Remarks { get; set; }
    public long PropertyUnitId { get; set; }
    public long CustomerId { get; set; }
    public bool IsActive { get; set; }
    public string PropertyUnitName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
}

public class CreateFlatHandoverDto
{
    public string HandoverNumber { get; set; } = string.Empty;
    public DateTime HandoverDate { get; set; }
    public string? KeyIssuedTo { get; set; }
    public HandoverStatus Status { get; set; }
    public string? Remarks { get; set; }
    public long PropertyUnitId { get; set; }
    public long CustomerId { get; set; }
}

public class UpdateFlatHandoverDto
{
    public long Id { get; set; }
    public string HandoverNumber { get; set; } = string.Empty;
    public DateTime HandoverDate { get; set; }
    public string? KeyIssuedTo { get; set; }
    public HandoverStatus Status { get; set; }
    public string? Remarks { get; set; }
    public long PropertyUnitId { get; set; }
    public long CustomerId { get; set; }
}
