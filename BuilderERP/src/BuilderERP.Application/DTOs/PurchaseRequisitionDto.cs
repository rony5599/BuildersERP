using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class PurchaseRequisitionDto
{
    public Guid Id { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime RequiredByDate { get; set; }
    public string? Description { get; set; }
    public decimal EstimatedAmount { get; set; }
    public RequisitionStatus Status { get; set; }
    public bool IsActive { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public List<PurchaseRequisitionDetailDto> Details { get; set; } = new();
}

public class PurchaseRequisitionDetailDto
{
    public Guid Id { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal EstimatedUnitPrice { get; set; }
    public decimal EstimatedAmount { get; set; }
    public string? Remarks { get; set; }
}

public class CreatePurchaseRequisitionDetailDto
{
    public Guid MaterialId { get; set; }
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal EstimatedUnitPrice { get; set; }
    public string? Remarks { get; set; }
}

public class CreatePurchaseRequisitionDto
{
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public DateTime RequiredByDate { get; set; }
    public string? Description { get; set; }
    public RequisitionStatus Status { get; set; } = RequisitionStatus.Draft;
    public Guid DepartmentId { get; set; }
    public Guid ProjectId { get; set; }
    public List<CreatePurchaseRequisitionDetailDto> Details { get; set; } = new();
}

public class UpdatePurchaseRequisitionDto
{
    public Guid Id { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime RequiredByDate { get; set; }
    public string? Description { get; set; }
    public RequisitionStatus Status { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid ProjectId { get; set; }
    public List<CreatePurchaseRequisitionDetailDto> Details { get; set; } = new();
}
