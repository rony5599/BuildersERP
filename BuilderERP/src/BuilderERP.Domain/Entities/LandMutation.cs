using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class LandMutation : BaseEntity
{
    public string MutationNumber { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string? KhatianNumber { get; set; }
    public string? DagNumber { get; set; }
    public DateTime MutationDate { get; set; } = DateTime.UtcNow;
    public MutationStatus Status { get; set; } = MutationStatus.Applied;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
