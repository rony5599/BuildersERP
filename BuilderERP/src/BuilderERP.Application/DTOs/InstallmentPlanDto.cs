using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class InstallmentPlanDto
{
    public long Id { get; set; }
    public decimal TotalAmount { get; set; }
    public int NumberOfInstallments { get; set; }
    public DateTime StartDate { get; set; }
    public decimal? InterestRatePercent { get; set; }
    public InstallmentPlanStatus Status { get; set; }
    public bool IsActive { get; set; }
    public long SaleAgreementId { get; set; }
    public string SaleAgreementNumber { get; set; } = string.Empty;
    public long? ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
}

public class CreateInstallmentPlanDto
{
    public decimal TotalAmount { get; set; }
    public int NumberOfInstallments { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public decimal? InterestRatePercent { get; set; }
    public InstallmentPlanStatus Status { get; set; } = InstallmentPlanStatus.Active;
    public long SaleAgreementId { get; set; }
}

public class UpdateInstallmentPlanDto
{
    public long Id { get; set; }
    public decimal TotalAmount { get; set; }
    public int NumberOfInstallments { get; set; }
    public DateTime StartDate { get; set; }
    public decimal? InterestRatePercent { get; set; }
    public InstallmentPlanStatus Status { get; set; }
    public long SaleAgreementId { get; set; }
}
