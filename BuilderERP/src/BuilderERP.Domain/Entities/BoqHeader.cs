using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class BoqHeader : BaseEntity
{
    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string BoqName { get; set; } = string.Empty;
    public int VersionNumber { get; set; } = 1;
    public decimal ContingencyPercent { get; set; } = 2m;
    public bool IsActive { get; set; } = true;
    public BoqStatus Status { get; set; } = BoqStatus.Draft;
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public ICollection<BoqItem> Items { get; set; } = new List<BoqItem>();
}
