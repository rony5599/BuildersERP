namespace BuilderERP.Application.DTOs;

public class SitePhotoDto
{
    public long Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime TakenDate { get; set; }
    public bool IsActive { get; set; }
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public long? DailyProgressId { get; set; }
    public DateTime? DailyProgressDate { get; set; }
}

public class CreateSitePhotoDto
{
    public string FilePath { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime TakenDate { get; set; } = DateTime.UtcNow;
    public long ProjectId { get; set; }
    public long? DailyProgressId { get; set; }
}

public class UpdateSitePhotoDto
{
    public long Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime TakenDate { get; set; }
    public long ProjectId { get; set; }
    public long? DailyProgressId { get; set; }
}
