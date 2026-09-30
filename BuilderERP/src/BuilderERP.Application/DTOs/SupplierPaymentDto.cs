using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class SupplierPaymentDto
{
    public long Id { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public string? PreparedBy { get; set; }
    public long? PoBillId { get; set; }
    public long? EwoBillId { get; set; }
    public string BillType => EwoBillId.HasValue ? "EWO" : "PO";
    public string BillNumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
}

public class CreateSupplierPaymentDto
{
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.BankTransfer;
    public string? ReferenceNumber { get; set; }
    public string? Remarks { get; set; }
    // Exactly one of the two is set: the payment is against a PO bill or an EWO bill.
    public long? PoBillId { get; set; }
    public long? EwoBillId { get; set; }
}

public class PayableBillDto
{
    public long Id { get; set; }
    public bool IsEwoBill { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    // PO number for a PO bill, work order number for an EWO bill.
    public string PONumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Outstanding => TotalAmount - PaidAmount;
}

public class SupplierLedgerRowDto
{
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
}

// Balance convention: positive = amount payable to the supplier (credits - debits).
public class SupplierLedgerDto
{
    public long SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string? SupplierAddress { get; set; }
    public string? SupplierPhone { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public SupplierLedgerSource Source { get; set; }
    public decimal OpeningBalance { get; set; }
    public List<SupplierLedgerRowDto> Rows { get; set; } = new();
    public decimal TotalDebit => Rows.Sum(r => r.Debit);
    public decimal TotalCredit => Rows.Sum(r => r.Credit);
    public decimal ClosingBalance => OpeningBalance + TotalCredit - TotalDebit;

    public string? CompanyName { get; set; }
    public string? PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }
}

// Which documents the supplier ledger includes. All gives the supplier's true balance;
// PO / EWO show one side (purchase returns belong to PO).
public enum SupplierLedgerSource
{
    All,
    Po,
    Ewo
}
