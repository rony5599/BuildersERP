namespace BuilderERP.Domain.Entities;

public class ItemPriceHistory : BaseEntity
{
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "BDT";
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

    public Guid MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
}
