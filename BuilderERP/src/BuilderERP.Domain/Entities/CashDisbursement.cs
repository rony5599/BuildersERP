using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

// Cash actually handed to a requester against an approved cash requisition.
public class CashDisbursement : BaseEntity
{
    public string DisbursementNumber { get; set; } = string.Empty;
    public DateTime DisbursementDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
    public string? ReferenceNumber { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public long CashRequisitionId { get; set; }
    public CashRequisition CashRequisition { get; set; } = null!;

    // Denormalized from the requisition so the requester ledger can filter without joins.
    public long RequesterEmployeeId { get; set; }
    public Employee RequesterEmployee { get; set; } = null!;
}
