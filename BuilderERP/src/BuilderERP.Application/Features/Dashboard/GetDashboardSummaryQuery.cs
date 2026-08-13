using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Dashboard;

public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDashboardSummaryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var projects = _unitOfWork.Repository<Project>().Query();
        var activeProjectCount = await projects.CountAsync(p => p.IsActive, cancellationToken);

        var milestones = _unitOfWork.Repository<Milestone>().Query();
        var totalMilestones = await milestones.CountAsync(cancellationToken);
        var achievedMilestones = await milestones.CountAsync(m => m.Status == MilestoneStatus.Achieved, cancellationToken);
        var progressPercent = totalMilestones == 0 ? 0 : Math.Round(achievedMilestones * 100m / totalMilestones, 1);

        var bookings = _unitOfWork.Repository<Booking>().Query();
        var salesTodayQuery = bookings.Where(b => b.BookingDate.Date == today);
        var salesTodayAmount = await salesTodayQuery.SumAsync(b => (decimal?)b.BookingAmount, cancellationToken) ?? 0;
        var salesTodayCount = await salesTodayQuery.CountAsync(cancellationToken);

        var receipts = _unitOfWork.Repository<Receipt>().Query();
        var cashInflow = await receipts.Where(r => r.PaymentDate >= monthStart)
            .SumAsync(r => (decimal?)r.AmountPaid, cancellationToken) ?? 0;

        var runningBills = _unitOfWork.Repository<RunningBill>().Query();
        var runningBillOutflow = await runningBills.Where(r => r.BillDate >= monthStart)
            .SumAsync(r => (decimal?)r.NetPayableAmount, cancellationToken) ?? 0;

        var salaries = _unitOfWork.Repository<Salary>().Query();
        var salaryOutflow = await salaries.Where(s => s.PeriodStart >= monthStart)
            .SumAsync(s => (decimal?)s.NetAmount, cancellationToken) ?? 0;

        var cashOutflow = runningBillOutflow + salaryOutflow;
        var netCashFlow = cashInflow - cashOutflow;

        var installments = _unitOfWork.Repository<Installment>().Query();
        var dueInstallments = installments.Where(i => i.Status != InstallmentStatus.Paid && i.DueDate <= today);
        var dueCollectionAmount = await dueInstallments
            .SumAsync(i => (decimal?)(i.DueAmount + i.PenaltyAmount - i.PaidAmount), cancellationToken) ?? 0;
        var dueCollectionCount = await dueInstallments.CountAsync(cancellationToken);

        var stocks = _unitOfWork.Repository<Stock>().Query();
        var inventoryQuantity = await stocks.SumAsync(s => (decimal?)s.QuantityOnHand, cancellationToken) ?? 0;

        var stockIssues = _unitOfWork.Repository<StockIssue>().Query();
        var materialConsumption = await stockIssues.Where(s => s.IssueDate >= monthStart)
            .SumAsync(s => (decimal?)s.Quantity, cancellationToken) ?? 0;

        var budgetLines = _unitOfWork.Repository<BudgetLine>().Query();
        var budgetedTotal = await budgetLines.SumAsync(b => (decimal?)b.BudgetedAmount, cancellationToken) ?? 0;
        var actualTotal = await budgetLines.SumAsync(b => (decimal?)b.ActualAmount, cancellationToken) ?? 0;
        var budgetVariancePercent = budgetedTotal == 0 ? 0 : Math.Round((actualTotal - budgetedTotal) * 100m / budgetedTotal, 1);

        var profitLoss = cashInflow - cashOutflow;

        var evaluations = _unitOfWork.Repository<PerformanceEvaluation>().Query();
        var hasEvaluations = await evaluations.AnyAsync(cancellationToken);
        var contractorScore = hasEvaluations
            ? await evaluations.AverageAsync(e => (double)(e.QualityScore + e.TimelinessScore + e.SafetyScore) / 3.0, cancellationToken)
            : 0;

        var attendances = _unitOfWork.Repository<Attendance>().Query();
        var monthAttendances = attendances.Where(a => a.AttendanceDate >= monthStart);
        var totalAttendanceRecords = await monthAttendances.CountAsync(cancellationToken);
        var presentRecords = await monthAttendances.CountAsync(a => a.Status == AttendanceStatus.Present, cancellationToken);
        var avgHours = totalAttendanceRecords == 0 ? 0 : await monthAttendances.AverageAsync(a => (double)a.HoursWorked, cancellationToken);
        var attendanceRate = totalAttendanceRecords == 0 ? 0 : Math.Round(presentRecords * 100m / totalAttendanceRecords, 1);

        return new DashboardSummaryDto
        {
            ProjectProgressPercent = progressPercent,
            ActiveProjectCount = activeProjectCount,
            SalesTodayAmount = salesTodayAmount,
            SalesTodayCount = salesTodayCount,
            CashInflowThisMonth = cashInflow,
            CashOutflowThisMonth = cashOutflow,
            NetCashFlowThisMonth = netCashFlow,
            DueCollectionAmount = dueCollectionAmount,
            DueCollectionCount = dueCollectionCount,
            InventoryQuantityOnHand = inventoryQuantity,
            MaterialConsumptionThisMonth = materialConsumption,
            BudgetedAmountTotal = budgetedTotal,
            ActualAmountTotal = actualTotal,
            BudgetVariancePercent = budgetVariancePercent,
            ProfitLossThisMonth = profitLoss,
            ContractorPerformanceAvgScore = (decimal)Math.Round(contractorScore, 1),
            EmployeeProductivityAvgHoursPerDay = (decimal)Math.Round(avgHours, 1),
            EmployeeAttendanceRatePercent = attendanceRate
        };
    }
}
