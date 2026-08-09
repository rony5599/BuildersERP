using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class SalaryDto
{
    public Guid Id { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal DaysWorked { get; set; }
    public decimal BasicAmount { get; set; }
    public decimal OvertimeAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal NetAmount { get; set; }
    public SalaryStatus Status { get; set; }
    public DateTime? PaymentDate { get; set; }
    public bool IsActive { get; set; }
    public Guid WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
}

public class CreateSalaryDto
{
    public DateTime PeriodStart { get; set; } = DateTime.UtcNow;
    public DateTime PeriodEnd { get; set; } = DateTime.UtcNow;
    public decimal DaysWorked { get; set; }
    public decimal BasicAmount { get; set; }
    public decimal OvertimeAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public SalaryStatus Status { get; set; } = SalaryStatus.Pending;
    public DateTime? PaymentDate { get; set; }
    public Guid WorkerId { get; set; }
}

public class UpdateSalaryDto
{
    public Guid Id { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal DaysWorked { get; set; }
    public decimal BasicAmount { get; set; }
    public decimal OvertimeAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public SalaryStatus Status { get; set; }
    public DateTime? PaymentDate { get; set; }
    public Guid WorkerId { get; set; }
}
