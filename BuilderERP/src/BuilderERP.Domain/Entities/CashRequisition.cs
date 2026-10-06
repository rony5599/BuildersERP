using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class CashRequisition : BaseEntity
{
    public string RequisitionNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public DateTime RequiredByDate { get; set; }
    public string? Description { get; set; }
    public decimal EstimatedAmount { get; set; }
    public RequisitionStatus Status { get; set; } = RequisitionStatus.Draft;
    public bool IsActive { get; set; } = true;
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }

    public long RequesterEmployeeId { get; set; }
    public Employee RequesterEmployee { get; set; } = null!;

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    public long DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ICollection<CashRequisitionDetail> Details { get; set; } = new List<CashRequisitionDetail>();
}
