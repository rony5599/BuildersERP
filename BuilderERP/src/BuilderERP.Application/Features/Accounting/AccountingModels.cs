using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Features.Accounting;

public sealed class LedgerAccountInput
{
    public long? Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public long? ParentId { get; set; }
    public bool AllowPosting { get; set; } = true;
    public bool IsActive { get; set; } = true;
}

public sealed class JournalVoucherInput
{
    public DateTime VoucherDate { get; set; } = DateTime.Today;
    public string Narration { get; set; } = string.Empty;
    public long? ProjectId { get; set; }
    public long? CostCenterId { get; set; }
    public List<JournalVoucherLineInput> Lines { get; set; } = [new(), new()];
}

public sealed class JournalVoucherLineInput
{
    public long LedgerAccountId { get; set; }
    public string? Description { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}

public sealed record TrialBalanceRow(long LedgerAccountId, string Code, string AccountName,
    AccountType AccountType, decimal Debit, decimal Credit, decimal Balance);

public sealed record JournalVoucherListItem(long Id, string VoucherNumber, DateTime VoucherDate,
    string Narration, JournalVoucherStatus Status, decimal Debit, decimal Credit);
