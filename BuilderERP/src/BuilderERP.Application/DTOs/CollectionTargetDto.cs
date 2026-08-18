namespace BuilderERP.Application.DTOs;

public class CollectionTargetDto
{
    public Guid Id { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TargetAmount { get; set; }
    public bool IsActive { get; set; }
    public Guid CollectionOfficerId { get; set; }
    public string CollectionOfficerName { get; set; } = string.Empty;
    public decimal ActualCollected { get; set; }
    public decimal AchievementPercent { get; set; }
}

public class CreateCollectionTargetDto
{
    public int Year { get; set; } = DateTime.UtcNow.Year;
    public int Month { get; set; } = DateTime.UtcNow.Month;
    public decimal TargetAmount { get; set; }
    public Guid CollectionOfficerId { get; set; }
}

public class UpdateCollectionTargetDto
{
    public Guid Id { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TargetAmount { get; set; }
    public Guid CollectionOfficerId { get; set; }
}
