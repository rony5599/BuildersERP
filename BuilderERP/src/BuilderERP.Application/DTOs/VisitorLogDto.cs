namespace BuilderERP.Application.DTOs;

public class VisitorLogDto
{
    public Guid Id { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? PurposeOfVisit { get; set; }
    public string? HostName { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? IdProofNumber { get; set; }
    public string? VehicleNumber { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsActive { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateVisitorLogDto
{
    public string VisitorName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? PurposeOfVisit { get; set; }
    public string? HostName { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? IdProofNumber { get; set; }
    public string? VehicleNumber { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateVisitorLogDto
{
    public Guid Id { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? PurposeOfVisit { get; set; }
    public string? HostName { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? IdProofNumber { get; set; }
    public string? VehicleNumber { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}
