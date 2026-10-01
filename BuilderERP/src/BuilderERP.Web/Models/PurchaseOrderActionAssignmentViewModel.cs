namespace BuilderERP.Web.Models;

public class PurchaseOrderActionAssignmentPageViewModel
{
    public List<PurchaseOrderActionAssignmentRowViewModel> Users { get; set; } = new();
}

public class PurchaseOrderActionAssignmentRowViewModel
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool CanDraftEdit { get; set; }
    public bool CanSubmit { get; set; }
    public bool CanRequestApproval { get; set; }
    public bool CanApprove { get; set; }
    public bool CanReject { get; set; }
    public bool CanCancel { get; set; }
}

public record PurchaseOrderWorkflowButtonViewModel(long Id, string WorkflowAction, string Label, string Css);

public class PurchaseOrderApprovalAssignmentViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Actions { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
}
