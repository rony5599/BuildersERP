using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class VendorQuotationDetail : BaseEntity
{
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal VatPercent { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public int? DeliveryDays { get; set; }

    public Guid VendorQuotationId { get; set; }
    public VendorQuotation VendorQuotation { get; set; } = null!;

    public Guid MaterialId { get; set; }
    public Material Material { get; set; } = null!;
}
