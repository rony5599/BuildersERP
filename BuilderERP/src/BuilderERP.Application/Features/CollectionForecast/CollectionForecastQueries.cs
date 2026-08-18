using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CollectionForecast;

internal static class CollectionForecastQueries
{
    public const decimal DefaultProbabilityPercent = 70m;

    public static async Task<List<Installment>> GetFilteredOutstandingInstallmentsAsync(
        IUnitOfWork unitOfWork,
        Guid? projectId,
        Guid? propertyUnitId,
        Guid? customerId,
        Guid? collectionOfficerId,
        CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<Installment>().Query()
            .Include(i => i.InstallmentPlan).ThenInclude(p => p.SaleAgreement).ThenInclude(a => a.Booking).ThenInclude(b => b.Customer)
            .Include(i => i.InstallmentPlan).ThenInclude(p => p.SaleAgreement).ThenInclude(a => a.Booking).ThenInclude(b => b.CollectionOfficer)
            .Include(i => i.InstallmentPlan).ThenInclude(p => p.SaleAgreement).ThenInclude(a => a.Booking).ThenInclude(b => b.PropertyUnit).ThenInclude(u => u!.Floor).ThenInclude(f => f.Tower).ThenInclude(t => t.Building).ThenInclude(b => b.Project)
            .Where(i => i.Status != InstallmentStatus.Paid)
            .AsQueryable();

        if (customerId.HasValue)
        {
            query = query.Where(i => i.InstallmentPlan.SaleAgreement.Booking.CustomerId == customerId.Value);
        }

        if (propertyUnitId.HasValue)
        {
            query = query.Where(i => i.InstallmentPlan.SaleAgreement.Booking.PropertyUnitId == propertyUnitId.Value);
        }

        if (collectionOfficerId.HasValue)
        {
            query = query.Where(i => i.InstallmentPlan.SaleAgreement.Booking.CollectionOfficerId == collectionOfficerId.Value);
        }

        if (projectId.HasValue)
        {
            query = query.Where(i => i.InstallmentPlan.SaleAgreement.Booking.PropertyUnit.Floor.Tower.Building.ProjectId == projectId.Value);
        }

        var installments = await query.ToListAsync(cancellationToken);
        return installments.Where(i => i.DueAmount + i.PenaltyAmount - i.PaidAmount > 0).ToList();
    }

    public static async Task<Dictionary<Guid, decimal>> GetCustomerCollectionProbabilitiesAsync(
        IUnitOfWork unitOfWork,
        IReadOnlyCollection<Guid> customerIds,
        DateTime today,
        CancellationToken cancellationToken)
    {
        if (customerIds.Count == 0)
        {
            return new Dictionary<Guid, decimal>();
        }

        var pastDueInstallments = await unitOfWork.Repository<Installment>().Query()
            .Include(i => i.InstallmentPlan).ThenInclude(p => p.SaleAgreement).ThenInclude(a => a.Booking)
            .Where(i => i.DueDate < today && customerIds.Contains(i.InstallmentPlan.SaleAgreement.Booking.CustomerId))
            .ToListAsync(cancellationToken);

        var result = new Dictionary<Guid, decimal>();
        foreach (var group in pastDueInstallments.GroupBy(i => i.InstallmentPlan.SaleAgreement.Booking.CustomerId))
        {
            var total = group.Count();
            var paid = group.Count(i => i.Status == InstallmentStatus.Paid);
            result[group.Key] = total == 0 ? DefaultProbabilityPercent : Math.Round(paid * 100m / total, 1);
        }

        foreach (var id in customerIds)
        {
            if (!result.ContainsKey(id))
            {
                result[id] = DefaultProbabilityPercent;
            }
        }

        return result;
    }

    public static async Task<List<Receipt>> GetFilteredReceiptsAsync(
        IUnitOfWork unitOfWork,
        DateTime from,
        DateTime to,
        Guid? projectId,
        Guid? propertyUnitId,
        Guid? customerId,
        Guid? collectionOfficerId,
        CancellationToken cancellationToken)
    {
        var receipts = await unitOfWork.Repository<Receipt>().Query()
            .Include(r => r.Installment).ThenInclude(i => i.InstallmentPlan).ThenInclude(p => p.SaleAgreement).ThenInclude(a => a.Booking).ThenInclude(b => b.PropertyUnit).ThenInclude(u => u!.Floor).ThenInclude(f => f.Tower).ThenInclude(t => t.Building)
            .Where(r => r.PaymentDate >= from && r.PaymentDate < to)
            .ToListAsync(cancellationToken);

        return receipts.Where(r =>
        {
            var booking = r.Installment?.InstallmentPlan?.SaleAgreement?.Booking;
            if (booking is null) return false;
            if (projectId.HasValue && booking.PropertyUnit?.Floor?.Tower?.Building?.ProjectId != projectId.Value) return false;
            if (propertyUnitId.HasValue && booking.PropertyUnitId != propertyUnitId.Value) return false;
            if (customerId.HasValue && booking.CustomerId != customerId.Value) return false;
            if (collectionOfficerId.HasValue && booking.CollectionOfficerId != collectionOfficerId.Value) return false;
            return true;
        }).ToList();
    }
}
