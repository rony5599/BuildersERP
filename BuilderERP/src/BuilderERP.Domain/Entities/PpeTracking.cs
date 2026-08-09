using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class PpeTracking : BaseEntity
{
    public Guid WorkerId { get; set; }
    public Worker Worker { get; set; } = null!;

    public PpeType PpeType { get; set; } = PpeType.Helmet;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public PpeStatus Status { get; set; } = PpeStatus.Issued;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;
}
