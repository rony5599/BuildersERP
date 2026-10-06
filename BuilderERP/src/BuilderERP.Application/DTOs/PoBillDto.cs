using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class PoBillDto
{
    public long Id { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? SupplierInvoiceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Remarks { get; set; }
    public PoBillStatus Status { get; set; }
    public bool IsActive { get; set; }
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? PreparedBy { get; set; }
    public long PurchaseOrderId { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public List<PoBillDetailDto> Details { get; set; } = new();
}

public class PoBillDetailDto
{
    public long Id { get; set; }
    public long PurchaseOrderDetailId { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal BilledQuantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal VatPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal LineTotal { get; set; }
}

public class PoBillLineInputDto
{
    public long PurchaseOrderDetailId { get; set; }
    public decimal BilledQuantity { get; set; }
}

// Used for both create (Id = 0) and edit. Prices always come from the PO, never from the client.
public class SavePoBillDto
{
    public long Id { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public string? SupplierInvoiceNumber { get; set; }
    public string? Remarks { get; set; }
    public PoBillStatus Status { get; set; } = PoBillStatus.Draft;
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public long PurchaseOrderId { get; set; }
    public List<PoBillLineInputDto> Details { get; set; } = new();
}

public class PoBillPrintDto
{
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? SupplierInvoiceNumber { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public DateTime PODate { get; set; }
    public PoBillStatus Status { get; set; }
    public string? Remarks { get; set; }
    public string? PreparedBy { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyAddress { get; set; }
    public string? CompanyPhone { get; set; }
    public string? CompanyEmail { get; set; }

    public string SupplierName { get; set; } = string.Empty;
    public string? SupplierAddress { get; set; }
    public string? SupplierPhone { get; set; }
    public string? SupplierEmail { get; set; }

    public string? PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }

    public List<PoBillPrintLineDto> Lines { get; set; } = new();

    public decimal Discount => Lines.Sum(l => l.Discount);
    public decimal Vat => Lines.Sum(l => l.Vat);
    public decimal Tax => Lines.Sum(l => l.Tax);
    public decimal Total => Lines.Sum(l => l.Amount);
    public decimal Subtotal => Total - Vat - Tax;
}

public class PoBillPrintLineDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Discount { get; set; }
    public decimal Vat { get; set; }
    public decimal Tax { get; set; }
    public decimal Amount { get; set; }
}

public class BillablePurchaseOrderDto
{
    public long Id { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
}

public class PoBillableLineDto
{
    public long PurchaseOrderDetailId { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal OrderedQuantity { get; set; }
    public decimal AlreadyBilledQuantity { get; set; }
    public decimal RemainingQuantity => OrderedQuantity - AlreadyBilledQuantity;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal VatPercent { get; set; }
    public decimal TaxPercent { get; set; }
}
