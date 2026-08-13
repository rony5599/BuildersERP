using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.ServiceTickets;

public record GetAllServiceTicketsQuery : IRequest<IReadOnlyList<ServiceTicketDto>>;

public class GetAllServiceTicketsQueryHandler : IRequestHandler<GetAllServiceTicketsQuery, IReadOnlyList<ServiceTicketDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllServiceTicketsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ServiceTicketDto>> Handle(GetAllServiceTicketsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<ServiceTicket>().Query()
            .Include(x => x.PropertyUnit)
            .OrderBy(x => x.TicketNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ServiceTicketDto>>(items);
    }
}
