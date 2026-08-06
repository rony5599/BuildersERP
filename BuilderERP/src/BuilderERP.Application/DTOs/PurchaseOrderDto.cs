using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ReceivedAmount { get; set; }
    public DateTime DeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public bool IsActive { get; set; }
    public Guid VendorQuotationId { get; set; }
    public string QuotationNumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
}

public class CreatePurchaseOrderDto
{
    public string PONumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public DateTime DeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public Guid VendorQuotationId { get; set; }
}

public class UpdatePurchaseOrderDto
{
    public Guid Id { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime DeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public Guid VendorQuotationId { get; set; }
}
