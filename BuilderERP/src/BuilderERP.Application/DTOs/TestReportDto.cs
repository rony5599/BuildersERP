using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class TestReportDto
{
    public Guid Id { get; set; }
    public string ReportNumber { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public DateTime TestDate { get; set; }
    public string? LabName { get; set; }
    public QcResult Result { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid? MaterialId { get; set; }
    public string? MaterialName { get; set; }
}

public class CreateTestReportDto
{
    public string ReportNumber { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public DateTime TestDate { get; set; } = DateTime.UtcNow;
    public string? LabName { get; set; }
    public QcResult Result { get; set; } = QcResult.Pending;
    public string FilePath { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public Guid? MaterialId { get; set; }
}

public class UpdateTestReportDto
{
    public Guid Id { get; set; }
    public string ReportNumber { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public DateTime TestDate { get; set; }
    public string? LabName { get; set; }
    public QcResult Result { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public Guid? MaterialId { get; set; }
}
