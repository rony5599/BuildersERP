using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PaymentReminders;

public record GetAllPaymentRemindersQuery(ReminderStatus? Status = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<PaymentReminderDto>>;

public class GetAllPaymentRemindersQueryHandler : IRequestHandler<GetAllPaymentRemindersQuery, PagedResult<PaymentReminderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPaymentRemindersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<PaymentReminderDto>> Handle(GetAllPaymentRemindersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PaymentReminder>().Query()
            .Include(r => r.Customer)
            .Include(r => r.Installment)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(r => r.Status == request.Status.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        query = query.OrderByDescending(r => r.ScheduledDate);

        var totalCount = await query.CountAsync(cancellationToken);
        var reminders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<PaymentReminderDto>>(reminders);
        return new PagedResult<PaymentReminderDto>(items, totalCount, page, pageSize);
    }
}
