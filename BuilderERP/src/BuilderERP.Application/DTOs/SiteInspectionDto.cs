using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class SiteInspectionDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime InspectionDate { get; set; }
    public string InspectedBy { get; set; } = string.Empty;
    public string? Location { get; set; }
    public QcResult Result { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
}

public class CreateSiteInspectionDto
{
    public Guid ProjectId { get; set; }
    public DateTime InspectionDate { get; set; } = DateTime.UtcNow;
    public string InspectedBy { get; set; } = string.Empty;
    public string? Location { get; set; }
    public QcResult Result { get; set; } = QcResult.Pending;
    public string? Remarks { get; set; }
}

public class UpdateSiteInspectionDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime InspectionDate { get; set; }
    public string InspectedBy { get; set; } = string.Empty;
    public string? Location { get; set; }
    public QcResult Result { get; set; }
    public string? Remarks { get; set; }
}
