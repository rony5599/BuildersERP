using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class AttendanceDto
{
    public Guid Id { get; set; }
    public Guid WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime AttendanceDate { get; set; }
    public AttendanceStatus Status { get; set; }
    public decimal HoursWorked { get; set; }
    public bool IsActive { get; set; }
}

public class CreateAttendanceDto
{
    public Guid WorkerId { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime AttendanceDate { get; set; } = DateTime.UtcNow;
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public decimal HoursWorked { get; set; } = 8;
}

public class UpdateAttendanceDto
{
    public Guid Id { get; set; }
    public Guid WorkerId { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public AttendanceStatus Status { get; set; }
    public decimal HoursWorked { get; set; }
}
