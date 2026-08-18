using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PaymentReminders;

public record CancelReminderCommand(Guid Id) : IRequest<bool>;

public class CancelReminderCommandHandler : IRequestHandler<CancelReminderCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelReminderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CancelReminderCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PaymentReminder>();
        var reminder = await repository.GetByIdAsync(request.Id);
        if (reminder is null)
        {
            return false;
        }

        reminder.Status = ReminderStatus.Cancelled;
        repository.Update(reminder);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
