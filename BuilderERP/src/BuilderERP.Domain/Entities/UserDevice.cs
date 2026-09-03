namespace BuilderERP.Domain.Entities;

public enum DeviceStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Revoked = 4,
    Blocked = 5
}

public class UserDevice : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public string DeviceId { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }
    public string? Browser { get; set; }
    public string? OperatingSystem { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }

    public DeviceStatus Status { get; set; } = DeviceStatus.Pending;
    public DateTime RequestedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ApprovedDate { get; set; }
    public Guid? ApprovedBy { get; set; }

    public DateTime? RejectedDate { get; set; }
    public Guid? RejectedBy { get; set; }

    public DateTime? RevokedDate { get; set; }
    public Guid? RevokedBy { get; set; }

    public DateTime? LastLoginDate { get; set; }
    public string? LastIPAddress { get; set; }
}
