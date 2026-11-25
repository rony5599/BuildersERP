namespace BuilderERP.Web.Models;

public class AccountingPostingAssignmentPageViewModel
{
    public List<AccountingPostingAssignmentRowViewModel> Users { get; set; } = new();
}

public class AccountingPostingAssignmentRowViewModel
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool CanPostPoBill { get; set; }
    public bool CanPostEwoBill { get; set; }
    public bool CanPostSupplierPayment { get; set; }
    public bool CanPostCustomerReceipt { get; set; }
    public bool CanPostManualJournal { get; set; }
    public bool CanReverse { get; set; }
}
