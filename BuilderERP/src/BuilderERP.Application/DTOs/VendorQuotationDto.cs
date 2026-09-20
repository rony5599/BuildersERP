using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class VendorQuotationDto
{
    public long Id { get; set; }
    public string QuotationNumber { get; set; } = string.Empty;
    public DateTime QuotationDate { get; set; }
    public decimal QuotedAmount { get; set; }
    public int DeliveryDays { get; set; }
    public VendorQuotationStatus Status { get; set; }
    public bool IsActive { get; set; }
    public long RfqId { get; set; }
    public string RfqNumber { get; set; } = string.Empty;
    public long SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public long PurchaseRequisitionId { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public List<VendorQuotationDetailDto> Details { get; set; } = new();
}

public class VendorQuotationDetailDto
{
    public long Id { get; set; }
    public long MaterialId { get; set; }
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
    public long MaterialId { get; set; }
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
    public DateTime QuotationDate { get; set; } = DateTime.UtcNow;
    public int DeliveryDays { get; set; }
    public VendorQuotationStatus Status { get; set; } = VendorQuotationStatus.Received;
    public long RfqId { get; set; }
    public long SupplierId { get; set; }
    public List<CreateVendorQuotationDetailDto> Details { get; set; } = new();
}

public class UpdateVendorQuotationDto
{
    public long Id { get; set; }
    public string QuotationNumber { get; set; } = string.Empty;
    public DateTime QuotationDate { get; set; }
    public int DeliveryDays { get; set; }
    public VendorQuotationStatus Status { get; set; }
    public long RfqId { get; set; }
    public long SupplierId { get; set; }
    public List<CreateVendorQuotationDetailDto> Details { get; set; } = new();
}

public class VendorQuotationPrintDto
{
    public string QuotationNumber { get; set; } = string.Empty;
    public DateTime QuotationDate { get; set; }
    public int DeliveryDays { get; set; }
    public VendorQuotationStatus Status { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyAddress { get; set; }
    public string? CompanyPhone { get; set; }
    public string? CompanyEmail { get; set; }

    public string SupplierName { get; set; } = string.Empty;
    public string? SupplierAddress { get; set; }
    public string? SupplierPhone { get; set; }
    public string? SupplierEmail { get; set; }

    public string RfqNumber { get; set; } = string.Empty;
    public string RequisitionNumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;

    public string? PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }

    public List<VendorQuotationPrintLineDto> Lines { get; set; } = new();

    public decimal Total => Lines.Sum(l => l.Amount);
}

public class VendorQuotationPrintLineDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}
