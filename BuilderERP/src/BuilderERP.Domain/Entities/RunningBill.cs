using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class RunningBill : BaseEntity
{
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; } = DateTime.UtcNow;
    public decimal WorkDoneAmount { get; set; }
    public decimal PreviousBillAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal NetPayableAmount { get; set; }
    public RunningBillStatus Status { get; set; } = RunningBillStatus.Draft;
    public bool IsActive { get; set; } = true;

    public Guid WorkOrderId { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;
}
