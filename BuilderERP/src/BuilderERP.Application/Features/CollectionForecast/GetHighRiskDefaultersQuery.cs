using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CollectionForecast;

public record GetHighRiskDefaultersQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<CustomerCollectionRiskDto>>;

public class GetHighRiskDefaultersQueryHandler : IRequestHandler<GetHighRiskDefaultersQuery, IReadOnlyList<CustomerCollectionRiskDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetHighRiskDefaultersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<CustomerCollectionRiskDto>> Handle(GetHighRiskDefaultersQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;

        var installments = await CollectionForecastQueries.GetFilteredOutstandingInstallmentsAsync(
            _unitOfWork, request.ProjectId, null, null, null, cancellationToken);

        var overdue = installments.Where(i => i.DueDate < today).ToList();
        var customerIds = overdue.Select(i => i.InstallmentPlan.SaleAgreement.Booking.CustomerId).Distinct().ToList();
        var probabilities = await CollectionForecastQueries.GetCustomerCollectionProbabilitiesAsync(_unitOfWork, customerIds, today, cancellationToken);

        var results = overdue
            .GroupBy(i => i.InstallmentPlan.SaleAgreement.Booking.CustomerId)
            .Select(group =>
            {
                var booking = group.First().InstallmentPlan.SaleAgreement.Booking;
                var overdueAmount = group.Sum(i => i.DueAmount + i.PenaltyAmount - i.PaidAmount);
                var overdueCount = group.Count();
                var maxDaysOverdue = group.Max(i => (int)(today - i.DueDate).TotalDays);
                var onTimeRate = probabilities.TryGetValue(group.Key, out var p) ? p : CollectionForecastQueries.DefaultProbabilityPercent;
                var isHighRisk = maxDaysOverdue > 90 || onTimeRate < 50 || overdueCount >= 3;

                return new CustomerCollectionRiskDto
                {
                    CustomerId = group.Key,
                    CustomerName = booking.Customer.FullName,
                    Phone = booking.Customer.Phone,
                    ProjectName = booking.PropertyUnit?.Floor?.Tower?.Building?.Project?.Name ?? string.Empty,
                    UnitNumber = booking.PropertyUnit?.UnitNumber ?? string.Empty,
                    OverdueAmount = overdueAmount,
                    OverdueInstallments = overdueCount,
                    MaxDaysOverdue = maxDaysOverdue,
                    OnTimePaymentRatePercent = onTimeRate,
                    IsHighRisk = isHighRisk
                };
            })
            .OrderByDescending(r => r.IsHighRisk)
            .ThenByDescending(r => r.OverdueAmount)
            .ToList();

        return results;
    }
}
