using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class CommonAreaBookingDto
{
    public Guid Id { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public string FacilityName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal Fee { get; set; }
    public CommonAreaBookingStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsActive { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateCommonAreaBookingDto
{
    public string BookingNumber { get; set; } = string.Empty;
    public string FacilityName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal Fee { get; set; }
    public CommonAreaBookingStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateCommonAreaBookingDto
{
    public Guid Id { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public string FacilityName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal Fee { get; set; }
    public CommonAreaBookingStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}
