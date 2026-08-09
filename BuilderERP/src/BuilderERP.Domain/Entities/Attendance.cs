using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Attendance : BaseEntity
{
    public Guid WorkerId { get; set; }
    public Worker Worker { get; set; } = null!;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public DateTime AttendanceDate { get; set; } = DateTime.UtcNow;
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public decimal HoursWorked { get; set; } = 8;
    public bool IsActive { get; set; } = true;
}
