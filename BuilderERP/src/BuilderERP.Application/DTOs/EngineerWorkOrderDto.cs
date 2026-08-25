using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class EngineerWorkOrderDto
{
    public Guid Id { get; set; }
    public string WorkOrderNo { get; set; } = string.Empty;
    public Guid EngineerWorkOrderRequisitionId { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public Guid? MotherWorkOrderId { get; set; }
    public int RevisionNo { get; set; }
    public bool IsLatestRevision { get; set; }
    public DateTime? RevisionDate { get; set; }
    public string? TermsAndCondition { get; set; }
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public EngineerWorkOrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid? PreviousWorkOrderId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<EngineerWorkOrderDetailDto> Details { get; set; } = new();
}

public class EngineerWorkOrderDetailDto
{
    public Guid Id { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal Qty { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string? Remarks { get; set; }
}

public class CreateEngineerWorkOrderDetailDto
{
    public Guid MaterialId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal Qty { get; set; }
    public decimal Rate { get; set; }
    public string? Remarks { get; set; }
}

public class CreateEngineerWorkOrderDto
{
    public Guid EngineerWorkOrderRequisitionId { get; set; }
    public Guid SupplierId { get; set; }
    public string? TermsAndCondition { get; set; }
    public EngineerWorkOrderStatus Status { get; set; } = EngineerWorkOrderStatus.Draft;
    public List<CreateEngineerWorkOrderDetailDto> Details { get; set; } = new();
}

public class UpdateEngineerWorkOrderDto
{
    public Guid Id { get; set; }
    public string WorkOrderNo { get; set; } = string.Empty;
    public Guid EngineerWorkOrderRequisitionId { get; set; }
    public Guid SupplierId { get; set; }
    public string? TermsAndCondition { get; set; }
    public EngineerWorkOrderStatus Status { get; set; }
    public List<CreateEngineerWorkOrderDetailDto> Details { get; set; } = new();
}
