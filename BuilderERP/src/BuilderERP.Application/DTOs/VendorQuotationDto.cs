using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class VendorQuotationDto
{
    public Guid Id { get; set; }
    public string QuotationNumber { get; set; } = string.Empty;
    public DateTime QuotationDate { get; set; }
    public decimal QuotedAmount { get; set; }
    public int DeliveryDays { get; set; }
    public VendorQuotationStatus Status { get; set; }
    public bool IsActive { get; set; }
    public Guid RfqId { get; set; }
    public string RfqNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public Guid PurchaseRequisitionId { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public List<VendorQuotationDetailDto> Details { get; set; } = new();
}

public class VendorQuotationDetailDto
{
    public Guid Id { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal VatPercent { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public int? DeliveryDays { get; set; }
}

public class CreateVendorQuotationDetailDto
{
    public Guid MaterialId { get; set; }
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal VatPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public int? DeliveryDays { get; set; }
}

public class CreateVendorQuotationDto
{
    public string QuotationNumber { get; set; } = string.Empty;
    public DateTime QuotationDate { get; set; } = DateTime.UtcNow;
    public int DeliveryDays { get; set; }
    public VendorQuotationStatus Status { get; set; } = VendorQuotationStatus.Received;
    public Guid RfqId { get; set; }
    public Guid SupplierId { get; set; }
    public List<CreateVendorQuotationDetailDto> Details { get; set; } = new();
}

public class UpdateVendorQuotationDto
{
    public Guid Id { get; set; }
    public string QuotationNumber { get; set; } = string.Empty;
    public DateTime QuotationDate { get; set; }
    public int DeliveryDays { get; set; }
    public VendorQuotationStatus Status { get; set; }
    public Guid RfqId { get; set; }
    public Guid SupplierId { get; set; }
    public List<CreateVendorQuotationDetailDto> Details { get; set; } = new();
}
