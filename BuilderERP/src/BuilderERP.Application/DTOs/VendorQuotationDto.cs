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
    public string SupplierName { get; set; } = string.Empty;
    public Guid PurchaseRequisitionId { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
}

public class CreateVendorQuotationDto
{
    public string QuotationNumber { get; set; } = string.Empty;
    public DateTime QuotationDate { get; set; } = DateTime.UtcNow;
    public decimal QuotedAmount { get; set; }
    public int DeliveryDays { get; set; }
    public VendorQuotationStatus Status { get; set; } = VendorQuotationStatus.Received;
    public Guid RfqId { get; set; }
}

public class UpdateVendorQuotationDto
{
    public Guid Id { get; set; }
    public string QuotationNumber { get; set; } = string.Empty;
    public DateTime QuotationDate { get; set; }
    public decimal QuotedAmount { get; set; }
    public int DeliveryDays { get; set; }
    public VendorQuotationStatus Status { get; set; }
    public Guid RfqId { get; set; }
}
