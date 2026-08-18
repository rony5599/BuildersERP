using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PaymentReminders;

public record GenerateRemindersCommand(int LookaheadDays = 7) : IRequest<int>;

public class GenerateRemindersCommandHandler : IRequestHandler<GenerateRemindersCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public GenerateRemindersCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(GenerateRemindersCommand request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var cutoff = today.AddDays(request.LookaheadDays);

        var candidateInstallments = await _unitOfWork.Repository<Installment>().Query()
            .Include(i => i.InstallmentPlan).ThenInclude(p => p.SaleAgreement).ThenInclude(a => a.Booking).ThenInclude(b => b.Customer)
            .Include(i => i.InstallmentPlan).ThenInclude(p => p.SaleAgreement).ThenInclude(a => a.Booking).ThenInclude(b => b.PropertyUnit)
            .Where(i => i.Status != InstallmentStatus.Paid && i.DueDate <= cutoff)
            .ToListAsync(cancellationToken);

        var existingPendingInstallmentIds = await _unitOfWork.Repository<PaymentReminder>().Query()
            .Where(r => r.Status == ReminderStatus.Pending && r.InstallmentId != null)
            .Select(r => r.InstallmentId!.Value)
            .ToListAsync(cancellationToken);

        var repository = _unitOfWork.Repository<PaymentReminder>();
        var created = 0;

        foreach (var installment in candidateInstallments)
        {
            if (existingPendingInstallmentIds.Contains(installment.Id))
            {
                continue;
            }

            var outstanding = installment.DueAmount + installment.PenaltyAmount - installment.PaidAmount;
            if (outstanding <= 0)
            {
                continue;
            }

            var booking = installment.InstallmentPlan?.SaleAgreement?.Booking;
            var customer = booking?.Customer;
            if (customer is null)
            {
                continue;
            }

            var unitNumber = booking?.PropertyUnit?.UnitNumber ?? string.Empty;
            var isOverdue = installment.DueDate < today;
            var message = isOverdue
                ? $"Dear {customer.FullName}, your installment of {outstanding:N2} for unit {unitNumber} was due on {installment.DueDate:yyyy-MM-dd} and is now overdue. Please make payment at your earliest convenience."
                : $"Dear {customer.FullName}, your installment of {outstanding:N2} for unit {unitNumber} is due on {installment.DueDate:yyyy-MM-dd}. Please arrange payment in time.";

            await repository.AddAsync(new PaymentReminder
            {
                CustomerId = customer.Id,
                InstallmentId = installment.Id,
                Channel = ReminderChannel.SMS,
                ScheduledDate = today,
                Message = message,
                Status = ReminderStatus.Pending
            });

            created++;
        }

        if (created > 0)
        {
            await _unitOfWork.SaveChangesAsync();
        }

        return created;
    }
}
