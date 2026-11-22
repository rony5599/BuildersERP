using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Features.Accounting;

public interface IAccountingService
{
    Task<IReadOnlyList<LedgerAccount>> GetAccountsAsync(bool postingOnly, CancellationToken cancellationToken);
    Task<long> SaveAccountAsync(LedgerAccountInput input, string userName, CancellationToken cancellationToken);
    Task<IReadOnlyList<JournalVoucherListItem>> GetVouchersAsync(CancellationToken cancellationToken);
    Task<JournalVoucher?> GetVoucherAsync(long id, CancellationToken cancellationToken);
    Task<long> CreateDraftAsync(JournalVoucherInput input, string userName, CancellationToken cancellationToken);
    Task PostAsync(long voucherId, string userName, CancellationToken cancellationToken);
    Task<long> ReverseAsync(long voucherId, DateTime reversalDate, string reason, string userName, CancellationToken cancellationToken);
    Task<IReadOnlyList<TrialBalanceRow>> GetTrialBalanceAsync(DateTime fromDate, DateTime toDate,
        long? projectId, long? costCenterId, CancellationToken cancellationToken);
}
