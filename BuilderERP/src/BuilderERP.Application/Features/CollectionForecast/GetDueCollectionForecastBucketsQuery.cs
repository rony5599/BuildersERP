using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CollectionForecast;

public record GetDueCollectionForecastBucketsQuery(
    string Granularity = "Monthly",
    long? ProjectId = null,
    long? PropertyUnitId = null,
    long? CustomerId = null,
    Guid? CollectionOfficerId = null) : IRequest<IReadOnlyList<DueCollectionForecastBucketDto>>;

public class GetDueCollectionForecastBucketsQueryHandler : IRequestHandler<GetDueCollectionForecastBucketsQuery, IReadOnlyList<DueCollectionForecastBucketDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDueCollectionForecastBucketsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<DueCollectionForecastBucketDto>> Handle(GetDueCollectionForecastBucketsQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var (periods, labelFormat) = BuildPeriods(request.Granularity, today);

        var installments = await CollectionForecastQueries.GetFilteredOutstandingInstallmentsAsync(
            _unitOfWork, request.ProjectId, request.PropertyUnitId, request.CustomerId, request.CollectionOfficerId, cancellationToken);

        var customerIds = installments.Select(i => i.InstallmentPlan.SaleAgreement.Booking.CustomerId).Distinct().ToList();
        var probabilities = await CollectionForecastQueries.GetCustomerCollectionProbabilitiesAsync(_unitOfWork, customerIds, today, cancellationToken);

        var rangeStart = periods.First().start;
        var rangeEnd = periods.Last().end;
        var receipts = await CollectionForecastQueries.GetFilteredReceiptsAsync(
            _unitOfWork, rangeStart, rangeEnd, request.ProjectId, request.PropertyUnitId, request.CustomerId, request.CollectionOfficerId, cancellationToken);

        var buckets = new List<DueCollectionForecastBucketDto>();
        foreach (var (start, end, label) in periods)
        {
            var periodInstallments = installments.Where(i => i.DueDate >= start && i.DueDate < end).ToList();
            var expected = periodInstallments.Sum(i => i.DueAmount + i.PenaltyAmount - i.PaidAmount);
            var likely = periodInstallments.Sum(i =>
            {
                var outstanding = i.DueAmount + i.PenaltyAmount - i.PaidAmount;
                var customerId = i.InstallmentPlan.SaleAgreement.Booking.CustomerId;
                var probability = probabilities.TryGetValue(customerId, out var p) ? p : CollectionForecastQueries.DefaultProbabilityPercent;
                return outstanding * probability / 100m;
            });
            var actual = receipts.Where(r => r.PaymentDate >= start && r.PaymentDate < end).Sum(r => r.AmountPaid);

            buckets.Add(new DueCollectionForecastBucketDto
            {
                PeriodLabel = label,
                PeriodStart = start,
                ExpectedAmount = expected,
                LikelyAmount = likely,
                ActualCollected = actual
            });
        }

        return buckets;
    }

    private static (List<(DateTime start, DateTime end, string label)> periods, string labelFormat) BuildPeriods(string granularity, DateTime today)
    {
        var periods = new List<(DateTime, DateTime, string)>();

        switch (granularity)
        {
            case "Daily":
                for (var i = 0; i < 14; i++)
                {
                    var day = today.AddDays(i);
                    periods.Add((day, day.AddDays(1), day.ToString("MMM dd")));
                }
                break;

            case "Weekly":
                var weekStart = today.AddDays(-(int)today.DayOfWeek);
                for (var i = 0; i < 8; i++)
                {
                    var start = weekStart.AddDays(i * 7);
                    periods.Add((start, start.AddDays(7), $"Wk of {start:MMM dd}"));
                }
                break;

            case "Yearly":
                for (var i = 0; i < 5; i++)
                {
                    var start = new DateTime(today.Year + i, 1, 1);
                    periods.Add((start, start.AddYears(1), start.Year.ToString()));
                }
                break;

            case "Monthly":
            default:
                var monthStart = new DateTime(today.Year, today.Month, 1);
                for (var i = 0; i < 12; i++)
                {
                    var start = monthStart.AddMonths(i);
                    periods.Add((start, start.AddMonths(1), start.ToString("MMM yyyy")));
                }
                break;
        }

        return (periods, granularity);
    }
}
