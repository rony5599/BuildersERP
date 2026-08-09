namespace BuilderERP.Domain.Entities;

public class OperatorAssignment : BaseEntity
{
    public DateTime AssignmentStartDate { get; set; } = DateTime.UtcNow;
    public DateTime? AssignmentEndDate { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;

    public Guid WorkerId { get; set; }
    public Worker Worker { get; set; } = null!;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
