using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class QuotationDto
{
    public long Id { get; set; }
    public decimal QuotedPrice { get; set; }
    public DateTime? ValidUntil { get; set; }
    public QuotationStatus Status { get; set; }
    public bool IsActive { get; set; }
    public long CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public long PropertyUnitId { get; set; }
    public string PropertyUnitNumber { get; set; } = string.Empty;
}

public class CreateQuotationDto
{
    public decimal QuotedPrice { get; set; }
    public DateTime? ValidUntil { get; set; }
    public QuotationStatus Status { get; set; } = QuotationStatus.Draft;
    public long CustomerId { get; set; }
    public long PropertyUnitId { get; set; }
}

public class UpdateQuotationDto
{
    public long Id { get; set; }
    public decimal QuotedPrice { get; set; }
    public DateTime? ValidUntil { get; set; }
    public QuotationStatus Status { get; set; }
    public long CustomerId { get; set; }
    public long PropertyUnitId { get; set; }
}
