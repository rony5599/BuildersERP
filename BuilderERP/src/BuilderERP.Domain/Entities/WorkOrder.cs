using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class WorkOrder : BaseEntity
{
    public string WorkOrderNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? CompletionDate { get; set; }
    public decimal Amount { get; set; }
    public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Draft;
    public bool IsActive { get; set; } = true;

    public long ContractorId { get; set; }
    public Contractor Contractor { get; set; } = null!;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
