namespace BuilderERP.Application.DTOs;

public class DueCollectionForecastSummaryDto
{
    public decimal TotalDue { get; set; }
    public decimal ExpectedThisMonth { get; set; }
    public decimal LikelyCollection { get; set; }
    public decimal ActualCollectedThisMonth { get; set; }
    public decimal CollectionAchievementPercent { get; set; }

    public decimal OverdueAmount { get; set; }
    public int OverdueCount { get; set; }

    public decimal CurrentDueAmount { get; set; }
    public int CurrentDueCount { get; set; }

    public decimal FutureDueAmount { get; set; }
    public int FutureDueCount { get; set; }

    public List<InstallmentDueRowDto> OverdueRows { get; set; } = new();
    public List<InstallmentDueRowDto> CurrentDueRows { get; set; } = new();
    public List<InstallmentDueRowDto> FutureDueRows { get; set; } = new();
}

public class InstallmentDueRowDto
{
    public Guid InstallmentId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public decimal OutstandingAmount { get; set; }
    public int DaysOverdue { get; set; }
    public string Bucket { get; set; } = string.Empty;
    public decimal CollectionProbabilityPercent { get; set; }
    public bool IsRescheduled { get; set; }
    public string? CollectionOfficerName { get; set; }
}

public class DueCollectionForecastBucketDto
{
    public string PeriodLabel { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public decimal ExpectedAmount { get; set; }
    public decimal LikelyAmount { get; set; }
    public decimal ActualCollected { get; set; }
}

public class CustomerCollectionRiskDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public decimal OverdueAmount { get; set; }
    public int OverdueInstallments { get; set; }
    public int MaxDaysOverdue { get; set; }
    public decimal OnTimePaymentRatePercent { get; set; }
    public bool IsHighRisk { get; set; }
}
