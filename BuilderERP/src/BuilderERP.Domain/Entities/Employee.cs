namespace BuilderERP.Domain.Entities;

public class Employee : BaseEntity
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string MobileNo { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    public long DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public long DesignationId { get; set; }
    public Designation Designation { get; set; } = null!;

    public long BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public long? ReportingToId { get; set; }
    public Employee? ReportingTo { get; set; }

    public Guid? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}
