using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class RunningBillDto
{
    public Guid Id { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; }
    public decimal WorkDoneAmount { get; set; }
    public decimal PreviousBillAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal NetPayableAmount { get; set; }
    public RunningBillStatus Status { get; set; }
    public bool IsActive { get; set; }
    public Guid WorkOrderId { get; set; }
    public string WorkOrderNumber { get; set; } = string.Empty;
}

public class CreateRunningBillDto
{
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; } = DateTime.UtcNow;
    public decimal WorkDoneAmount { get; set; }
    public decimal PreviousBillAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public RunningBillStatus Status { get; set; } = RunningBillStatus.Draft;
    public Guid WorkOrderId { get; set; }
}

public class UpdateRunningBillDto
{
    public Guid Id { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; }
    public decimal WorkDoneAmount { get; set; }
    public decimal PreviousBillAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public RunningBillStatus Status { get; set; }
    public Guid WorkOrderId { get; set; }
}
