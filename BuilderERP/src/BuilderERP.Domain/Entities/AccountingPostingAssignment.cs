namespace BuilderERP.Domain.Entities;

public class AccountingPostingAssignment : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public bool CanPostPoBill { get; set; }
    public bool CanPostEwoBill { get; set; }
    public bool CanPostSupplierPayment { get; set; }
    public bool CanPostCustomerReceipt { get; set; }
    public bool CanPostManualJournal { get; set; }
    public bool CanReverse { get; set; }
}
