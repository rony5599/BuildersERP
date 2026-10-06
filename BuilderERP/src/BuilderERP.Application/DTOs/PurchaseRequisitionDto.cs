using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class PurchaseRequisitionDto
{
    public long Id { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime RequiredByDate { get; set; }
    public string? Description { get; set; }
    public decimal EstimatedAmount { get; set; }
    public RequisitionStatus Status { get; set; }
    public bool IsActive { get; set; }
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public List<PurchaseRequisitionDetailDto> Details { get; set; } = new();
}

public class PurchaseRequisitionDetailDto
{
    public long Id { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal EstimatedUnitPrice { get; set; }
    public decimal EstimatedAmount { get; set; }
    public string? Remarks { get; set; }
}

public class CreatePurchaseRequisitionDetailDto
{
    public long MaterialId { get; set; }
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
    public long DepartmentId { get; set; }
    public long ProjectId { get; set; }
    public List<CreatePurchaseRequisitionDetailDto> Details { get; set; } = new();
}

public class UpdatePurchaseRequisitionDto
{
    public long Id { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime RequiredByDate { get; set; }
    public string? Description { get; set; }
    public RequisitionStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public long DepartmentId { get; set; }
    public long ProjectId { get; set; }
    public List<CreatePurchaseRequisitionDetailDto> Details { get; set; } = new();
}

public class PurchaseRequisitionPrintDto
{
    public string RequisitionNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime RequiredByDate { get; set; }
    public RequisitionStatus Status { get; set; }
    public string? Description { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyAddress { get; set; }
    public string? CompanyPhone { get; set; }
    public string? CompanyEmail { get; set; }

    public string DepartmentName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;

    public string? PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }

    public List<PurchaseRequisitionPrintLineDto> Lines { get; set; } = new();

    public decimal Total => Lines.Sum(l => l.Amount);
}

public class PurchaseRequisitionPrintLineDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string? Remarks { get; set; }
}
