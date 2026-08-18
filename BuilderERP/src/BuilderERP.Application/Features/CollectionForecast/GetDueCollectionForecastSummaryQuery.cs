using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CollectionForecast;

public record GetDueCollectionForecastSummaryQuery(
    Guid? ProjectId = null,
    Guid? PropertyUnitId = null,
    Guid? CustomerId = null,
    Guid? CollectionOfficerId = null) : IRequest<DueCollectionForecastSummaryDto>;

public class GetDueCollectionForecastSummaryQueryHandler : IRequestHandler<GetDueCollectionForecastSummaryQuery, DueCollectionForecastSummaryDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDueCollectionForecastSummaryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DueCollectionForecastSummaryDto> Handle(GetDueCollectionForecastSummaryQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1);

        var installments = await CollectionForecastQueries.GetFilteredOutstandingInstallmentsAsync(
            _unitOfWork, request.ProjectId, request.PropertyUnitId, request.CustomerId, request.CollectionOfficerId, cancellationToken);

        var customerIds = installments.Select(i => i.InstallmentPlan.SaleAgreement.Booking.CustomerId).Distinct().ToList();
        var probabilities = await CollectionForecastQueries.GetCustomerCollectionProbabilitiesAsync(_unitOfWork, customerIds, today, cancellationToken);

        var summary = new DueCollectionForecastSummaryDto();

        foreach (var installment in installments)
        {
            var booking = installment.InstallmentPlan.SaleAgreement.Booking;
            var outstanding = installment.DueAmount + installment.PenaltyAmount - installment.PaidAmount;
            var probability = probabilities.TryGetValue(booking.CustomerId, out var p) ? p : CollectionForecastQueries.DefaultProbabilityPercent;

            var row = new InstallmentDueRowDto
            {
                InstallmentId = installment.Id,
                CustomerId = booking.CustomerId,
                CustomerName = booking.Customer.FullName,
                CustomerPhone = booking.Customer.Phone,
                ProjectName = booking.PropertyUnit?.Floor?.Tower?.Building?.Project?.Name ?? string.Empty,
                UnitNumber = booking.PropertyUnit?.UnitNumber ?? string.Empty,
                DueDate = installment.DueDate,
                OutstandingAmount = outstanding,
                DaysOverdue = installment.DueDate < today ? (int)(today - installment.DueDate).TotalDays : 0,
                CollectionProbabilityPercent = probability,
                IsRescheduled = installment.IsRescheduled,
                CollectionOfficerName = booking.CollectionOfficer?.FullName
            };

            summary.TotalDue += outstanding;

            if (installment.DueDate < today)
            {
                row.Bucket = "Overdue";
                summary.OverdueAmount += outstanding;
                summary.OverdueCount++;
                summary.OverdueRows.Add(row);
            }
            else if (installment.DueDate < monthEnd)
            {
                row.Bucket = "Current";
                summary.CurrentDueAmount += outstanding;
                summary.CurrentDueCount++;
                summary.CurrentDueRows.Add(row);
            }
            else
            {
                row.Bucket = "Future";
                summary.FutureDueAmount += outstanding;
                summary.FutureDueCount++;
                summary.FutureDueRows.Add(row);
            }

            if (installment.DueDate >= monthStart && installment.DueDate < monthEnd)
            {
                summary.ExpectedThisMonth += outstanding;
                summary.LikelyCollection += outstanding * probability / 100m;
            }
        }

        summary.OverdueRows = summary.OverdueRows.OrderByDescending(r => r.DaysOverdue).ToList();
        summary.CurrentDueRows = summary.CurrentDueRows.OrderBy(r => r.DueDate).ToList();
        summary.FutureDueRows = summary.FutureDueRows.OrderBy(r => r.DueDate).ToList();

        var filteredReceipts = await CollectionForecastQueries.GetFilteredReceiptsAsync(
            _unitOfWork, monthStart, monthEnd, request.ProjectId, request.PropertyUnitId, request.CustomerId, request.CollectionOfficerId, cancellationToken);

        summary.ActualCollectedThisMonth = filteredReceipts.Sum(r => r.AmountPaid);
        summary.CollectionAchievementPercent = summary.ExpectedThisMonth == 0
            ? 0
            : Math.Round(summary.ActualCollectedThisMonth * 100m / summary.ExpectedThisMonth, 1);

        return summary;
    }
}
