namespace BuilderERP.Application.DTOs;

public class SitePhotoDto
{
    public Guid Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime TakenDate { get; set; }
    public bool IsActive { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid? DailyProgressId { get; set; }
    public DateTime? DailyProgressDate { get; set; }
}

public class CreateSitePhotoDto
{
    public string FilePath { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime TakenDate { get; set; } = DateTime.UtcNow;
    public Guid ProjectId { get; set; }
    public Guid? DailyProgressId { get; set; }
}

public class UpdateSitePhotoDto
{
    public Guid Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime TakenDate { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? DailyProgressId { get; set; }
}
