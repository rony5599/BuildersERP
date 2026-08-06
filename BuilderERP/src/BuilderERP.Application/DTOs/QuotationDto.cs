using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class QuotationDto
{
    public Guid Id { get; set; }
    public decimal QuotedPrice { get; set; }
    public DateTime? ValidUntil { get; set; }
    public QuotationStatus Status { get; set; }
    public bool IsActive { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid PropertyUnitId { get; set; }
    public string PropertyUnitNumber { get; set; } = string.Empty;
}

public class CreateQuotationDto
{
    public decimal QuotedPrice { get; set; }
    public DateTime? ValidUntil { get; set; }
    public QuotationStatus Status { get; set; } = QuotationStatus.Draft;
    public Guid CustomerId { get; set; }
    public Guid PropertyUnitId { get; set; }
}

public class UpdateQuotationDto
{
    public Guid Id { get; set; }
    public decimal QuotedPrice { get; set; }
    public DateTime? ValidUntil { get; set; }
    public QuotationStatus Status { get; set; }
    public Guid CustomerId { get; set; }
    public Guid PropertyUnitId { get; set; }
}
