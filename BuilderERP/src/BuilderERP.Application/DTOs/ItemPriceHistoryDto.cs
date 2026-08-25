namespace BuilderERP.Application.DTOs;

public class ItemPriceHistoryDto
{
    public long Id { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public long SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal VatPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal NetPrice { get; set; }
    public decimal MinimumOrderQuantity { get; set; }
    public int LeadTimeDays { get; set; }
    public bool IsContractPrice { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string SourceDocumentType { get; set; } = string.Empty;
    public string SourceDocumentNumber { get; set; } = string.Empty;
}
