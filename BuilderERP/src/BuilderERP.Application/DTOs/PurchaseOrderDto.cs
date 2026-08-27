using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class PurchaseOrderDto
{
    public long Id { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ReceivedAmount { get; set; }
    public DateTime DeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public bool IsActive { get; set; }
    public long VendorQuotationId { get; set; }
    public string QuotationNumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public List<PurchaseOrderDetailDto> Details { get; set; } = new();
}

public class PurchaseOrderDetailDto
{
    public long Id { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal OrderedQuantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal VatPercent { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal RemainingQuantity => OrderedQuantity - ReceivedQuantity;
}

public class CreatePurchaseOrderDetailDto
{
    public long MaterialId { get; set; }
    public decimal OrderedQuantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal VatPercent { get; set; }
    public decimal TaxPercent { get; set; }
}

public class CreatePurchaseOrderDto
{
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime DeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public long VendorQuotationId { get; set; }
    public List<CreatePurchaseOrderDetailDto> Details { get; set; } = new();
}

public class UpdatePurchaseOrderDto
{
    public long Id { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public long VendorQuotationId { get; set; }
    public List<CreatePurchaseOrderDetailDto> Details { get; set; } = new();
}

public class PurchaseOrderPrintDto
{
    public string PONumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyAddress { get; set; }
    public string? CompanyPhone { get; set; }
    public string? CompanyEmail { get; set; }

    public string SupplierName { get; set; } = string.Empty;
    public string? SupplierAddress { get; set; }
    public string? SupplierPhone { get; set; }
    public string? SupplierEmail { get; set; }

    public List<PurchaseOrderPrintLineDto> Lines { get; set; } = new();

    public decimal Subtotal => Lines.Sum(l => l.Amount);
    public decimal Total => Subtotal;
}

public class PurchaseOrderPrintLineDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}
