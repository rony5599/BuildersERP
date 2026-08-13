namespace BuilderERP.Application.DTOs;

public class DashboardSummaryDto
{
    public decimal ProjectProgressPercent { get; set; }
    public int ActiveProjectCount { get; set; }

    public decimal SalesTodayAmount { get; set; }
    public int SalesTodayCount { get; set; }

    public decimal CashInflowThisMonth { get; set; }
    public decimal CashOutflowThisMonth { get; set; }
    public decimal NetCashFlowThisMonth { get; set; }

    public decimal DueCollectionAmount { get; set; }
    public int DueCollectionCount { get; set; }

    public decimal InventoryQuantityOnHand { get; set; }
    public decimal MaterialConsumptionThisMonth { get; set; }

    public decimal BudgetedAmountTotal { get; set; }
    public decimal ActualAmountTotal { get; set; }
    public decimal BudgetVariancePercent { get; set; }

    public decimal ProfitLossThisMonth { get; set; }

    public decimal ContractorPerformanceAvgScore { get; set; }

    public decimal EmployeeProductivityAvgHoursPerDay { get; set; }
    public decimal EmployeeAttendanceRatePercent { get; set; }
}
