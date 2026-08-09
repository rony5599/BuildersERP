using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class PpeTrackingDto
{
    public Guid Id { get; set; }
    public Guid WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public PpeType PpeType { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public PpeStatus Status { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
}

public class CreatePpeTrackingDto
{
    public Guid WorkerId { get; set; }
    public PpeType PpeType { get; set; } = PpeType.Helmet;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public PpeStatus Status { get; set; } = PpeStatus.Issued;
    public string? Remarks { get; set; }
}

public class UpdatePpeTrackingDto
{
    public Guid Id { get; set; }
    public Guid WorkerId { get; set; }
    public PpeType PpeType { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public PpeStatus Status { get; set; }
    public string? Remarks { get; set; }
}
