using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PaymentReminders;

public record GetAllPaymentRemindersQuery(ReminderStatus? Status = null) : IRequest<IReadOnlyList<PaymentReminderDto>>;

public class GetAllPaymentRemindersQueryHandler : IRequestHandler<GetAllPaymentRemindersQuery, IReadOnlyList<PaymentReminderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPaymentRemindersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PaymentReminderDto>> Handle(GetAllPaymentRemindersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PaymentReminder>().Query()
            .Include(r => r.Customer)
            .Include(r => r.Installment)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(r => r.Status == request.Status.Value);
        }

        var reminders = await query.OrderByDescending(r => r.ScheduledDate).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<PaymentReminderDto>>(reminders);
    }
}
