namespace BuilderERP.Web.Models;

public class EngineerWorkOrderActionAssignmentPageViewModel
{
    public List<EngineerWorkOrderActionAssignmentRowViewModel> Users { get; set; } = new();
}

public class EngineerWorkOrderActionAssignmentRowViewModel
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

public record EngineerWorkOrderWorkflowButtonViewModel(long Id, string WorkflowAction, string Label, string Css);

public class EngineerWorkOrderApprovalAssignmentViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Actions { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
}
