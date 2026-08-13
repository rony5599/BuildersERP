using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class UtilityBillDto
{
    public Guid Id { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public UtilityType UtilityType { get; set; }
    public DateTime BillingMonth { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public UtilityBillStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
    public bool IsActive { get; set; }
    public string PropertyUnitName { get; set; } = string.Empty;
}

public class CreateUtilityBillDto
{
    public string BillNumber { get; set; } = string.Empty;
    public UtilityType UtilityType { get; set; }
    public DateTime BillingMonth { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public UtilityBillStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
}

public class UpdateUtilityBillDto
{
    public Guid Id { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public UtilityType UtilityType { get; set; }
    public DateTime BillingMonth { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public UtilityBillStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid PropertyUnitId { get; set; }
}
