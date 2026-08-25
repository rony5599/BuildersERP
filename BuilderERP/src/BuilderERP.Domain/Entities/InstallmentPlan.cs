using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class InstallmentPlan : BaseEntity
{
    public decimal TotalAmount { get; set; }
    public int NumberOfInstallments { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public decimal? InterestRatePercent { get; set; }
    public InstallmentPlanStatus Status { get; set; } = InstallmentPlanStatus.Active;
    public bool IsActive { get; set; } = true;

    public long SaleAgreementId { get; set; }
    public SaleAgreement SaleAgreement { get; set; } = null!;

    public ICollection<Installment> Installments { get; set; } = new List<Installment>();
}
