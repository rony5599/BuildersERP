using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class LandDocumentDto
{
    public Guid Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public LandDocumentType LandDocumentType { get; set; }
    public string? MouzaName { get; set; }
    public string? JlNumber { get; set; }
    public string? KhatianNumber { get; set; }
    public string? DagNumber { get; set; }
    public decimal AreaInDecimal { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsActive { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateLandDocumentDto
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public LandDocumentType LandDocumentType { get; set; }
    public string? MouzaName { get; set; }
    public string? JlNumber { get; set; }
    public string? KhatianNumber { get; set; }
    public string? DagNumber { get; set; }
    public decimal AreaInDecimal { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateLandDocumentDto
{
    public Guid Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public LandDocumentType LandDocumentType { get; set; }
    public string? MouzaName { get; set; }
    public string? JlNumber { get; set; }
    public string? KhatianNumber { get; set; }
    public string? DagNumber { get; set; }
    public decimal AreaInDecimal { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public string? Remarks { get; set; }
    public Guid ProjectId { get; set; }
}
