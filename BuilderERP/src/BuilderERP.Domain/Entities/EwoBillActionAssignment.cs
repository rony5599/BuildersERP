namespace BuilderERP.Domain.Entities;

public class EwoBillActionAssignment : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public bool CanDraftEdit { get; set; }
    public bool CanSubmit { get; set; }
    public bool CanRequestApproval { get; set; }
    public bool CanApprove { get; set; }
    public bool CanReject { get; set; }
    public bool CanCancel { get; set; }
}
