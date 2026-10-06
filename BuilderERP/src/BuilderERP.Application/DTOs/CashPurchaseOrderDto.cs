using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class CashPurchaseOrderDto
{
    public long Id { get; set; }
    public string CPONumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ReceivedAmount { get; set; }
    public DateTime DeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public bool IsActive { get; set; }
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? TermsOfPayment { get; set; }
    public string? DispatchedThrough { get; set; }
    public string? Destination { get; set; }
    public string? Remarks { get; set; }
    public long SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public long CashRequisitionId { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public List<CashPurchaseOrderDetailDto> Details { get; set; } = new();
}

public class CashPurchaseOrderDetailDto
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

public class CreateCashPurchaseOrderDetailDto
{
    public long MaterialId { get; set; }
    public decimal OrderedQuantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal VatPercent { get; set; }
    public decimal TaxPercent { get; set; }
}

public class CreateCashPurchaseOrderDto
{
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime DeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public string? TermsOfPayment { get; set; }
    public string? DispatchedThrough { get; set; }
    public string? Destination { get; set; }
    public string? Remarks { get; set; }
    public long SupplierId { get; set; }
    public long CashRequisitionId { get; set; }
    public List<CreateCashPurchaseOrderDetailDto> Details { get; set; } = new();
}

public class UpdateCashPurchaseOrderDto
{
    public long Id { get; set; }
    public string CPONumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? TermsOfPayment { get; set; }
    public string? DispatchedThrough { get; set; }
    public string? Destination { get; set; }
    public string? Remarks { get; set; }
    public long SupplierId { get; set; }
    public long CashRequisitionId { get; set; }
    public List<CreateCashPurchaseOrderDetailDto> Details { get; set; } = new();
}

public class CashPurchaseOrderPrintDto
{
    public string CPONumber { get; set; } = string.Empty;
    public string RequisitionNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyAddress { get; set; }
    public string? CompanyPhone { get; set; }
    public string? CompanyEmail { get; set; }

    public string SupplierName { get; set; } = string.Empty;
    public string? SupplierAddress { get; set; }
    public string? SupplierPhone { get; set; }
    public string? SupplierEmail { get; set; }

    public string? TermsOfPayment { get; set; }
    public string? DispatchedThrough { get; set; }
    public string? Destination { get; set; }
    public string? Remarks { get; set; }

    public string? PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }

    public List<CashPurchaseOrderPrintLineDto> Lines { get; set; } = new();

    public decimal Subtotal => Lines.Sum(l => l.Amount);
    public decimal Total => Subtotal;
}

public class CashPurchaseOrderPrintLineDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}
