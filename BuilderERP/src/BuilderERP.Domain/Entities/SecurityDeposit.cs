using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class SecurityDeposit : BaseEntity
{
    public Guid ContractorId { get; set; }
    public Contractor Contractor { get; set; } = null!;

    public Guid? WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }

    public decimal DepositAmount { get; set; }
    public DateTime DepositDate { get; set; } = DateTime.UtcNow;
    public DateTime? RefundDate { get; set; }
    public SecurityDepositStatus Status { get; set; } = SecurityDepositStatus.Held;
    public bool IsActive { get; set; } = true;
}
