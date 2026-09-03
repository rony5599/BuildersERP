using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.DTOs;

public class DeviceApprovalDto
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }
    public string? Browser { get; set; }
    public string? OperatingSystem { get; set; }
    public string? IPAddress { get; set; }
    public DateTime RequestedDate { get; set; }
    public DeviceStatus Status { get; set; }
}
