using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Salary : BaseEntity
{
    public DateTime PeriodStart { get; set; } = DateTime.UtcNow;
    public DateTime PeriodEnd { get; set; } = DateTime.UtcNow;
    public decimal DaysWorked { get; set; }
    public decimal BasicAmount { get; set; }
    public decimal OvertimeAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal NetAmount { get; set; }
    public SalaryStatus Status { get; set; } = SalaryStatus.Pending;
    public DateTime? PaymentDate { get; set; }
    public bool IsActive { get; set; } = true;

    public long WorkerId { get; set; }
    public Worker Worker { get; set; } = null!;
}
