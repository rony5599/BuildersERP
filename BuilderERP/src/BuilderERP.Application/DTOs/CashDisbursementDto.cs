using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class CashDisbursementDto
{
    public long Id { get; set; }
    public string DisbursementNumber { get; set; } = string.Empty;
    public DateTime DisbursementDate { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public string? PreparedBy { get; set; }
    public long CashRequisitionId { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
}

public class CreateCashDisbursementDto
{
    public DateTime DisbursementDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
    public string? ReferenceNumber { get; set; }
    public string? Remarks { get; set; }
    public long CashRequisitionId { get; set; }
}

public class DisbursableRequisitionDto
{
    public long Id { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public decimal EstimatedAmount { get; set; }
    public decimal DisbursedAmount { get; set; }
    public decimal Remaining => EstimatedAmount - DisbursedAmount;
}
