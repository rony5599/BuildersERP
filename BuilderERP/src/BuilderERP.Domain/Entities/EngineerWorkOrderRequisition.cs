using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class EngineerWorkOrderRequisition : BaseEntity
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

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ICollection<EngineerWorkOrderRequisitionDetail> Details { get; set; } = new List<EngineerWorkOrderRequisitionDetail>();
}
