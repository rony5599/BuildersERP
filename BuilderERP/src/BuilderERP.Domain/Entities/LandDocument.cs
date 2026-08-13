using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class LandDocument : BaseEntity
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public LandDocumentType LandDocumentType { get; set; } = LandDocumentType.Deed;
    public string? MouzaName { get; set; }
    public string? JlNumber { get; set; }
    public string? KhatianNumber { get; set; }
    public string? DagNumber { get; set; }
    public decimal AreaInDecimal { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
