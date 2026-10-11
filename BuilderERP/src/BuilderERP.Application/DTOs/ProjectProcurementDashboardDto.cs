namespace BuilderERP.Application.DTOs;

public class ProjectProcurementDashboardDto
{
    public long? ProjectId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public List<ProjectLookupDto> Projects { get; set; } = new();
    public List<ProjectProcurementRowDto> Rows { get; set; } = new();
    public List<MonthlyProcurementCommitmentDto> MonthlyCommitments { get; set; } = new();

    public int PurchaseRequisitionCount => Rows.Sum(x => x.PurchaseRequisitionCount);
    public decimal PurchaseRequisitionAmount => Rows.Sum(x => x.PurchaseRequisitionAmount);
    public int PurchaseOrderCount => Rows.Sum(x => x.PurchaseOrderCount);
    public decimal PurchaseOrderAmount => Rows.Sum(x => x.PurchaseOrderAmount);
    public int EwoRequisitionCount => Rows.Sum(x => x.EwoRequisitionCount);
    public decimal EwoRequisitionAmount => Rows.Sum(x => x.EwoRequisitionAmount);
    public int EwoCount => Rows.Sum(x => x.EwoCount);
    public decimal EwoAmount => Rows.Sum(x => x.EwoAmount);
    public int CashRequisitionCount => Rows.Sum(x => x.CashRequisitionCount);
    public decimal CashRequisitionAmount => Rows.Sum(x => x.CashRequisitionAmount);
    public int CashPurchaseOrderCount => Rows.Sum(x => x.CashPurchaseOrderCount);
    public decimal CashPurchaseOrderAmount => Rows.Sum(x => x.CashPurchaseOrderAmount);
    public int PoBillCount => Rows.Sum(x => x.PoBillCount);
    public decimal PoBillAmount => Rows.Sum(x => x.PoBillAmount);
    public int EwoBillCount => Rows.Sum(x => x.EwoBillCount);
    public decimal EwoBillAmount => Rows.Sum(x => x.EwoBillAmount);
    public int CashPoBillCount => Rows.Sum(x => x.CashPoBillCount);
    public decimal CashPoBillAmount => Rows.Sum(x => x.CashPoBillAmount);
    public decimal BoqBudgetAmount => Rows.Sum(x => x.BoqBudgetAmount);
    public decimal TotalCommittedAmount => PurchaseOrderAmount + EwoAmount + CashPurchaseOrderAmount;
    public decimal TotalBilledAmount => PoBillAmount + EwoBillAmount + CashPoBillAmount;
    public decimal BudgetVariance => BoqBudgetAmount - TotalCommittedAmount;
    public bool IsBudgetExceeded => TotalCommittedAmount > BoqBudgetAmount;
}

public class MonthlyProcurementCommitmentDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal PurchaseOrderAmount { get; set; }
    public decimal EwoAmount { get; set; }
    public decimal CashPurchaseOrderAmount { get; set; }
    public decimal PieAmount => PurchaseOrderAmount + EwoAmount + CashPurchaseOrderAmount;
}

public class ProjectLookupDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ProjectProcurementRowDto
{
    public long ProjectId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public int PurchaseRequisitionCount { get; set; }
    public decimal PurchaseRequisitionAmount { get; set; }
    public int PurchaseOrderCount { get; set; }
    public decimal PurchaseOrderAmount { get; set; }
    public int EwoRequisitionCount { get; set; }
    public decimal EwoRequisitionAmount { get; set; }
    public int EwoCount { get; set; }
    public decimal EwoAmount { get; set; }
    public int CashRequisitionCount { get; set; }
    public decimal CashRequisitionAmount { get; set; }
    public int CashPurchaseOrderCount { get; set; }
    public decimal CashPurchaseOrderAmount { get; set; }
    public int PoBillCount { get; set; }
    public decimal PoBillAmount { get; set; }
    public int EwoBillCount { get; set; }
    public decimal EwoBillAmount { get; set; }
    public int CashPoBillCount { get; set; }
    public decimal CashPoBillAmount { get; set; }
    public decimal BoqBudgetAmount { get; set; }
    public decimal TotalCommittedAmount => PurchaseOrderAmount + EwoAmount + CashPurchaseOrderAmount;
    public decimal TotalBilledAmount => PoBillAmount + EwoBillAmount + CashPoBillAmount;
    public decimal BudgetVariance => BoqBudgetAmount - TotalCommittedAmount;
    public decimal BudgetUtilizationPercent => BoqBudgetAmount == 0
        ? (TotalCommittedAmount > 0 ? 100m : 0m)
        : TotalCommittedAmount * 100m / BoqBudgetAmount;
    public bool IsBudgetExceeded => TotalCommittedAmount > BoqBudgetAmount;
}
