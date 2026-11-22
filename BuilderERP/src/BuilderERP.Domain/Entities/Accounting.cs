namespace BuilderERP.Domain.Entities;

public enum AccountType
{
    Asset = 1,
    Liability = 2,
    Equity = 3,
    Revenue = 4,
    Expense = 5
}

public enum JournalVoucherStatus
{
    Draft = 1,
    Posted = 2,
    Reversed = 3
}

public class LedgerAccount : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public long? ParentId { get; set; }
    public LedgerAccount? Parent { get; set; }
    public ICollection<LedgerAccount> Children { get; set; } = new List<LedgerAccount>();
    public bool AllowPosting { get; set; } = true;
    public bool IsActive { get; set; } = true;
}

public class JournalVoucher : BaseEntity
{
    public string VoucherNumber { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public string Narration { get; set; } = string.Empty;
    public JournalVoucherStatus Status { get; set; } = JournalVoucherStatus.Draft;
    public string SourceType { get; set; } = "Manual";
    public long? SourceId { get; set; }
    public long? ProjectId { get; set; }
    public Project? Project { get; set; }
    public long? CostCenterId { get; set; }
    public CostCenter? CostCenter { get; set; }
    public DateTime? PostedAt { get; set; }
    public string? PostedBy { get; set; }
    public long? ReversalVoucherId { get; set; }
    public JournalVoucher? ReversalVoucher { get; set; }
    public ICollection<JournalVoucherLine> Lines { get; set; } = new List<JournalVoucherLine>();
}

public class JournalVoucherLine : BaseEntity
{
    public long JournalVoucherId { get; set; }
    public JournalVoucher JournalVoucher { get; set; } = null!;
    public long LedgerAccountId { get; set; }
    public LedgerAccount LedgerAccount { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public int LineNumber { get; set; }
}
