using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PaymentReminders;

public record MarkReminderSentCommand(long Id, ReminderChannel Channel) : IRequest<bool>;

public class MarkReminderSentCommandHandler : IRequestHandler<MarkReminderSentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkReminderSentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(MarkReminderSentCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PaymentReminder>();
        var reminder = await repository.GetByIdAsync(request.Id);
        if (reminder is null)
        {
            return false;
        }

        reminder.Channel = request.Channel;
        reminder.Status = ReminderStatus.Sent;
        reminder.SentDate = DateTime.UtcNow;
        repository.Update(reminder);

        var communicationType = request.Channel switch
        {
            ReminderChannel.SMS => CommunicationType.SMS,
            ReminderChannel.WhatsApp => CommunicationType.WhatsApp,
            ReminderChannel.Email => CommunicationType.Email,
            _ => CommunicationType.Other
        };

        await _unitOfWork.Repository<CustomerCommunication>().AddAsync(new CustomerCommunication
        {
            CustomerId = reminder.CustomerId,
            CommunicationDate = DateTime.UtcNow,
            Type = communicationType,
            Subject = "Payment reminder",
            Notes = reminder.Message
        });

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
