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
}

public class CreatePurchaseRequisitionDto
{
    public string RequisitionNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public DateTime RequiredByDate { get; set; }
    public string? Description { get; set; }
    public decimal EstimatedAmount { get; set; }
    public RequisitionStatus Status { get; set; } = RequisitionStatus.Draft;
    public Guid DepartmentId { get; set; }
}

public class UpdatePurchaseRequisitionDto
{
    public Guid Id { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime RequiredByDate { get; set; }
    public string? Description { get; set; }
    public decimal EstimatedAmount { get; set; }
    public RequisitionStatus Status { get; set; }
    public Guid DepartmentId { get; set; }
}
