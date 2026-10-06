using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class CashPoBillDto
{
    public long Id { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; }
    public string? MemoNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Remarks { get; set; }
    public PoBillStatus Status { get; set; }
    public bool IsActive { get; set; }
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? PreparedBy { get; set; }
    public long CashPurchaseOrderId { get; set; }
    public string CPONumber { get; set; } = string.Empty;
    public string RequisitionNumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public long RequesterEmployeeId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public List<CashPoBillDetailDto> Details { get; set; } = new();
}

public class CashPoBillDetailDto
{
    public long Id { get; set; }
    public long CashPurchaseOrderDetailId { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal BilledQuantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal VatPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal LineTotal { get; set; }
}

public class CashPoBillLineInputDto
{
    public long CashPurchaseOrderDetailId { get; set; }
    public decimal BilledQuantity { get; set; }
}

// Used for both create (Id = 0) and edit. Prices always come from the Cash PO, never from the client.
public class SaveCashPoBillDto
{
    public long Id { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; } = DateTime.UtcNow;
    public string? MemoNumber { get; set; }
    public string? Remarks { get; set; }
    public PoBillStatus Status { get; set; } = PoBillStatus.Draft;
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public long CashPurchaseOrderId { get; set; }
    public List<CashPoBillLineInputDto> Details { get; set; } = new();
}

public class CashPoBillPrintDto
{
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; }
    public string? MemoNumber { get; set; }
    public string CPONumber { get; set; } = string.Empty;
    public DateTime CPODate { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public string? RequesterCode { get; set; }
    public string? RequesterMobile { get; set; }
    public string? ProjectName { get; set; }
    public PoBillStatus Status { get; set; }
    public string? Remarks { get; set; }
    public string? PreparedBy { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyAddress { get; set; }
    public string? CompanyPhone { get; set; }
    public string? CompanyEmail { get; set; }

    public string SupplierName { get; set; } = string.Empty;
    public string? SupplierAddress { get; set; }
    public string? SupplierPhone { get; set; }
    public string? SupplierEmail { get; set; }

    public string? PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }

    public List<PoBillPrintLineDto> Lines { get; set; } = new();

    public decimal Discount => Lines.Sum(l => l.Discount);
    public decimal Vat => Lines.Sum(l => l.Vat);
    public decimal Tax => Lines.Sum(l => l.Tax);
    public decimal Total => Lines.Sum(l => l.Amount);
}

public class BillableCashPurchaseOrderDto
{
    public long Id { get; set; }
    public string CPONumber { get; set; } = string.Empty;
    public string RequisitionNumber { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
}

public class CashPoBillableLineDto
{
    public long CashPurchaseOrderDetailId { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal OrderedQuantity { get; set; }
    public decimal AlreadyBilledQuantity { get; set; }
    public decimal RemainingQuantity => OrderedQuantity - AlreadyBilledQuantity;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal VatPercent { get; set; }
    public decimal TaxPercent { get; set; }
}

public class RequesterLedgerRowDto
{
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
}

// Balance convention: positive = cash issued to the requester that is not yet accounted for
// by approved cash bills (debits - credits). Negative = requester spent more than was issued,
// i.e. the company owes the requester.
public class RequesterLedgerDto
{
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string? EmployeeCode { get; set; }
    public string? EmployeeMobile { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal OpeningBalance { get; set; }
    public List<RequesterLedgerRowDto> Rows { get; set; } = new();
    public decimal TotalDebit => Rows.Sum(r => r.Debit);
    public decimal TotalCredit => Rows.Sum(r => r.Credit);
    public decimal ClosingBalance => OpeningBalance + TotalDebit - TotalCredit;

    public static string Describe(decimal balance) =>
        balance < 0 ? $"{Math.Abs(balance):N2} owed to requester"
        : balance > 0 ? $"{balance:N2} held by requester"
        : "0.00 settled";

    public string? CompanyName { get; set; }
    public string? PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }
}

public class RequesterOptionDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
