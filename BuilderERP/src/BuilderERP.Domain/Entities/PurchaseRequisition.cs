using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class PurchaseRequisition : BaseEntity
{
    public string RequisitionNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public DateTime RequiredByDate { get; set; }
    public string? Description { get; set; }
    public decimal EstimatedAmount { get; set; }
    public RequisitionStatus Status { get; set; } = RequisitionStatus.Draft;
    public bool IsActive { get; set; } = true;

    public long DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ICollection<PurchaseRequisitionDetail> Details { get; set; } = new List<PurchaseRequisitionDetail>();
}
