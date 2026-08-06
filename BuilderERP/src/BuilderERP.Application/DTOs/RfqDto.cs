using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class RfqDto
{
    public Guid Id { get; set; }
    public string RfqNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ClosingDate { get; set; }
    public RfqStatus Status { get; set; }
    public bool IsActive { get; set; }
    public Guid PurchaseRequisitionId { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
}

public class CreateRfqDto
{
    public string RfqNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime ClosingDate { get; set; }
    public RfqStatus Status { get; set; } = RfqStatus.Sent;
    public Guid PurchaseRequisitionId { get; set; }
    public Guid SupplierId { get; set; }
}

public class UpdateRfqDto
{
    public Guid Id { get; set; }
    public string RfqNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ClosingDate { get; set; }
    public RfqStatus Status { get; set; }
    public Guid PurchaseRequisitionId { get; set; }
    public Guid SupplierId { get; set; }
}
