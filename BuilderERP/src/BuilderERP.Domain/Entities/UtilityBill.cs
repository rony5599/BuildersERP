using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class UtilityBill : BaseEntity
{
    public string BillNumber { get; set; } = string.Empty;
    public UtilityType UtilityType { get; set; } = UtilityType.Electricity;
    public DateTime BillingMonth { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; } = DateTime.UtcNow;
    public DateTime? PaidDate { get; set; }
    public UtilityBillStatus Status { get; set; } = UtilityBillStatus.Unpaid;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid PropertyUnitId { get; set; }
    public PropertyUnit PropertyUnit { get; set; } = null!;
}
