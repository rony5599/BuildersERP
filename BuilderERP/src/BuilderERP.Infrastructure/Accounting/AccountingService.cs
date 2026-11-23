using System.Data;
using BuilderERP.Application.Features.Accounting;
using BuilderERP.Domain.Entities;
using BuilderERP.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Infrastructure.Accounting;

public sealed class AccountingService(AppDbContext db) : IAccountingService
{
    public async Task<IReadOnlyList<LedgerAccount>> GetAccountsAsync(bool postingOnly, CancellationToken ct) =>
        await db.LedgerAccounts.AsNoTracking()
            .Where(a => !a.IsDeleted && a.IsActive && (!postingOnly || a.AllowPosting))
            .OrderBy(a => a.Code).ToListAsync(ct);

    public async Task<long> SaveAccountAsync(LedgerAccountInput input, string userName, CancellationToken ct)
    {
        var code = input.Code.Trim();
        if (await db.LedgerAccounts.AnyAsync(a => a.Code == code && a.Id != input.Id, ct))
            throw new InvalidOperationException($"Account code '{code}' already exists.");
        if (input.ParentId == input.Id) throw new InvalidOperationException("An account cannot be its own parent.");

        LedgerAccount account;
        if (input.Id.HasValue)
        {
            account = await db.LedgerAccounts.FindAsync([input.Id.Value], ct)
                ?? throw new KeyNotFoundException("Ledger account not found.");
            account.ModifiedAt = DateTime.UtcNow;
            account.ModifiedBy = userName;
        }
        else
        {
            account = new LedgerAccount { CreatedBy = userName };
            db.LedgerAccounts.Add(account);
        }
        account.Code = code;
        account.Name = input.Name.Trim();
        account.AccountType = input.AccountType;
        account.ParentId = input.ParentId;
        account.AllowPosting = input.AllowPosting;
        account.IsActive = input.IsActive;
        await db.SaveChangesAsync(ct);
        return account.Id;
    }

    public async Task<IReadOnlyList<JournalVoucherListItem>> GetVouchersAsync(CancellationToken ct) =>
        await db.JournalVouchers.AsNoTracking().Where(v => !v.IsDeleted)
            .OrderByDescending(v => v.VoucherDate).ThenByDescending(v => v.Id)
            .Select(v => new JournalVoucherListItem(v.Id, v.VoucherNumber, v.VoucherDate, v.Narration,
                v.Status, v.Lines.Sum(l => l.Debit), v.Lines.Sum(l => l.Credit))).ToListAsync(ct);

    public Task<JournalVoucher?> GetVoucherAsync(long id, CancellationToken ct) =>
        db.JournalVouchers.AsNoTracking().Include(v => v.Project).Include(v => v.CostCenter)
            .Include(v => v.Lines).ThenInclude(l => l.LedgerAccount).FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task<long> CreateDraftAsync(JournalVoucherInput input, string userName, CancellationToken ct)
    {
        ValidateLines(input.Lines);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var datePrefix = $"JV-{input.VoucherDate:yyyyMM}-";
        var last = await db.JournalVouchers.Where(v => v.VoucherNumber.StartsWith(datePrefix))
            .OrderByDescending(v => v.VoucherNumber).Select(v => v.VoucherNumber).FirstOrDefaultAsync(ct);
        var sequence = last is null || !int.TryParse(last[datePrefix.Length..], out var n) ? 1 : n + 1;
        var voucher = new JournalVoucher
        {
            VoucherNumber = $"{datePrefix}{sequence:00000}", VoucherDate = input.VoucherDate.Date,
            Narration = input.Narration.Trim(), ProjectId = input.ProjectId, CostCenterId = input.CostCenterId,
            CreatedBy = userName, Lines = input.Lines.Select((l, i) => new JournalVoucherLine
            {
                LedgerAccountId = l.LedgerAccountId, Description = l.Description?.Trim(), Debit = l.Debit,
                Credit = l.Credit, LineNumber = i + 1, CreatedBy = userName
            }).ToList()
        };
        db.JournalVouchers.Add(voucher);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return voucher.Id;
    }

    public async Task PostAsync(long voucherId, string userName, CancellationToken ct)
    {
        await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.usp_Accounting_PostJournalVoucher @VoucherId={voucherId}, @PostedBy={userName}", ct);
        db.ChangeTracker.Clear();
    }

    public async Task<long> ReverseAsync(long voucherId, DateTime reversalDate, string reason, string userName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new InvalidOperationException("A reversal reason is required.");
        var output = new SqlParameter("@ReversalVoucherId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
        await db.Database.ExecuteSqlRawAsync(
            "EXEC dbo.usp_Accounting_ReverseJournalVoucher @VoucherId, @ReversalDate, @Reason, @ReversedBy, @ReversalVoucherId OUTPUT",
            [new SqlParameter("@VoucherId", voucherId), new SqlParameter("@ReversalDate", reversalDate.Date),
             new SqlParameter("@Reason", reason.Trim()), new SqlParameter("@ReversedBy", userName), output], ct);
        db.ChangeTracker.Clear();
        return Convert.ToInt64(output.Value);
    }

    public async Task PostSourceAsync(string sourceType, long sourceId, string userName, CancellationToken ct)
    {
        if (sourceId <= 0) throw new ArgumentOutOfRangeException(nameof(sourceId));
        if (sourceType is not ("PoBill" or "EwoBill" or "SupplierPayment"))
            throw new ArgumentException("Unsupported accounting source type.", nameof(sourceType));

        await db.Database.ExecuteSqlRawAsync(
            "EXEC dbo.usp_Accounting_PostSource @SourceType, @SourceId, @PostedBy",
            [new SqlParameter("@SourceType", sourceType), new SqlParameter("@SourceId", sourceId),
             new SqlParameter("@PostedBy", userName)], ct);
        db.ChangeTracker.Clear();
    }

    public async Task AutoReverseSourceAsync(string sourceType, long sourceId, DateTime reversalDate, string reason, string userName, CancellationToken ct)
    {
        await db.Database.ExecuteSqlRawAsync(
            "EXEC dbo.usp_Accounting_AutoReverseSource @SourceType, @SourceId, @ReversalDate, @Reason, @ReversedBy",
            [new SqlParameter("@SourceType", sourceType), new SqlParameter("@SourceId", sourceId),
             new SqlParameter("@ReversalDate", reversalDate.Date), new SqlParameter("@Reason", reason),
             new SqlParameter("@ReversedBy", userName)], ct);
        db.ChangeTracker.Clear();
    }

    public async Task<IReadOnlyList<TrialBalanceRow>> GetTrialBalanceAsync(DateTime fromDate, DateTime toDate,
        long? projectId, long? costCenterId, CancellationToken ct)
    {
        var connection = (SqlConnection)db.Database.GetDbConnection();
        await using var command = new SqlCommand("dbo.usp_Accounting_TrialBalance", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@FromDate", fromDate.Date);
        command.Parameters.AddWithValue("@ToDate", toDate.Date);
        command.Parameters.AddWithValue("@ProjectId", (object?)projectId ?? DBNull.Value);
        command.Parameters.AddWithValue("@CostCenterId", (object?)costCenterId ?? DBNull.Value);
        if (connection.State != ConnectionState.Open) await connection.OpenAsync(ct);
        var rows = new List<TrialBalanceRow>();
        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
            rows.Add(new(reader.GetInt64(0), reader.GetString(1), reader.GetString(2),
                (AccountType)reader.GetInt32(3), reader.GetDecimal(4), reader.GetDecimal(5), reader.GetDecimal(6)));
        return rows;
    }

    private static void ValidateLines(IReadOnlyCollection<JournalVoucherLineInput> lines)
    {
        if (lines.Count < 2) throw new InvalidOperationException("A voucher requires at least two lines.");
        if (lines.Any(l => l.LedgerAccountId <= 0 || l.Debit < 0 || l.Credit < 0 || (l.Debit > 0) == (l.Credit > 0)))
            throw new InvalidOperationException("Each line must contain one posting account and either a debit or a credit.");
        if (lines.Sum(l => l.Debit) != lines.Sum(l => l.Credit) || lines.Sum(l => l.Debit) <= 0)
            throw new InvalidOperationException("Total debit and credit must be equal and greater than zero.");
    }
}
