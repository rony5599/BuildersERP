namespace BuilderERP.Application.DTOs;

public class EmployeeDto
{
    public long Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string MobileNo { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; }

    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;

    public long DesignationId { get; set; }
    public string DesignationName { get; set; } = string.Empty;

    public long BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;

    public long? ReportingToId { get; set; }
    public string? ReportingToName { get; set; }

    public Guid? UserId { get; set; }
}

public class CreateEmployeeDto
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string MobileNo { get; set; } = string.Empty;
    public string? Email { get; set; }
    public long DepartmentId { get; set; }
    public long DesignationId { get; set; }
    public long BranchId { get; set; }
    public long? ReportingToId { get; set; }
    public Guid? UserId { get; set; }
}

public class UpdateEmployeeDto
{
    public long Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string MobileNo { get; set; } = string.Empty;
    public string? Email { get; set; }
    public long DepartmentId { get; set; }
    public long DesignationId { get; set; }
    public long BranchId { get; set; }
    public long? ReportingToId { get; set; }
    public Guid? UserId { get; set; }
}
