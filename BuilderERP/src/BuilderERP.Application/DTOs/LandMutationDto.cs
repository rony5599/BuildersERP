using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class LandMutationDto
{
    public long Id { get; set; }
    public string MutationNumber { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string? KhatianNumber { get; set; }
    public string? DagNumber { get; set; }
    public DateTime MutationDate { get; set; }
    public MutationStatus Status { get; set; }
    public string? Remarks { get; set; }
    public long ProjectId { get; set; }
    public bool IsActive { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateLandMutationDto
{
    public string MutationNumber { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string? KhatianNumber { get; set; }
    public string? DagNumber { get; set; }
    public DateTime MutationDate { get; set; }
    public MutationStatus Status { get; set; }
    public string? Remarks { get; set; }
    public long ProjectId { get; set; }
}

public class UpdateLandMutationDto
{
    public long Id { get; set; }
    public string MutationNumber { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string? KhatianNumber { get; set; }
    public string? DagNumber { get; set; }
    public DateTime MutationDate { get; set; }
    public MutationStatus Status { get; set; }
    public string? Remarks { get; set; }
    public long ProjectId { get; set; }
}
