using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class EngineerWorkOrderDto
{
    public long Id { get; set; }
    public string WorkOrderNo { get; set; } = string.Empty;
    public long EngineerWorkOrderRequisitionId { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public long? MotherWorkOrderId { get; set; }
    public int RevisionNo { get; set; }
    public bool IsLatestRevision { get; set; }
    public DateTime? RevisionDate { get; set; }
    public string? TermsAndCondition { get; set; }
    public long SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public EngineerWorkOrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ReceivedAmount { get; set; }
    public long? PreviousWorkOrderId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<EngineerWorkOrderDetailDto> Details { get; set; } = new();
    public List<EngineerWorkOrderPaymentHeadDto> PaymentHeads { get; set; } = new();
}

public class EngineerWorkOrderPaymentHeadDto
{
    public string HeadName { get; set; } = string.Empty;
    public decimal Percent { get; set; }
}

public class EngineerWorkOrderDetailDto
{
    public long Id { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal Qty { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal RemainingQuantity => Qty - ReceivedQuantity;
    public string? Remarks { get; set; }
}

public class CreateEngineerWorkOrderDetailDto
{
    public long MaterialId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal Qty { get; set; }
    public decimal Rate { get; set; }
    public string? Remarks { get; set; }
}

public class CreateEngineerWorkOrderDto
{
    public long EngineerWorkOrderRequisitionId { get; set; }
    public long SupplierId { get; set; }
    public string? TermsAndCondition { get; set; }
    public EngineerWorkOrderStatus Status { get; set; } = EngineerWorkOrderStatus.Draft;
    public List<CreateEngineerWorkOrderDetailDto> Details { get; set; } = new();
    public List<EngineerWorkOrderPaymentHeadDto> PaymentHeads { get; set; } = new();
}

public class UpdateEngineerWorkOrderDto
{
    public long Id { get; set; }
    public string WorkOrderNo { get; set; } = string.Empty;
    public long EngineerWorkOrderRequisitionId { get; set; }
    public long SupplierId { get; set; }
    public string? TermsAndCondition { get; set; }
    public EngineerWorkOrderStatus Status { get; set; }
    public List<CreateEngineerWorkOrderDetailDto> Details { get; set; } = new();
    public List<EngineerWorkOrderPaymentHeadDto> PaymentHeads { get; set; } = new();
}

// Payment heads can still be changed on an approved work order until its first bill is raised.
public class SaveEngineerWorkOrderPaymentHeadsDto
{
    public long EngineerWorkOrderId { get; set; }
    public List<EngineerWorkOrderPaymentHeadDto> PaymentHeads { get; set; } = new();
}

public class EngineerWorkOrderPrintDto
{
    public string WorkOrderNo { get; set; } = string.Empty;
    public string RequisitionNumber { get; set; } = string.Empty;
    public int RevisionNo { get; set; }
    public DateTime OrderDate { get; set; }
    public EngineerWorkOrderStatus Status { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyAddress { get; set; }
    public string? CompanyPhone { get; set; }
    public string? CompanyEmail { get; set; }

    public string SupplierName { get; set; } = string.Empty;
    public string? SupplierAddress { get; set; }
    public string? SupplierPhone { get; set; }
    public string? SupplierEmail { get; set; }

    public string? TermsAndCondition { get; set; }

    public string? PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }

    public List<EngineerWorkOrderPrintLineDto> Lines { get; set; } = new();
    public List<EngineerWorkOrderPaymentHeadDto> PaymentHeads { get; set; } = new();

    public decimal Subtotal => Lines.Sum(l => l.Amount);
    public decimal Total => Subtotal;
}

public class EngineerWorkOrderPrintLineDto
{
    public string Description { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string? Remarks { get; set; }
}
