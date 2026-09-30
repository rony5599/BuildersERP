using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class SupplierPayment : BaseEntity
{
    public string PaymentNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.BankTransfer;
    public string? ReferenceNumber { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    // Exactly one of PoBillId / EwoBillId is set (enforced by a check constraint).
    public long? PoBillId { get; set; }
    public PoBill? PoBill { get; set; }

    public long? EwoBillId { get; set; }
    public EwoBill? EwoBill { get; set; }

    // Denormalized from the bill's PO / work order so the supplier ledger can filter without deep joins.
    public long SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
}
