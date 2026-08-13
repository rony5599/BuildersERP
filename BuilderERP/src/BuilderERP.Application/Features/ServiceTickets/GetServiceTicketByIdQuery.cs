using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.ServiceTickets;

public record GetServiceTicketByIdQuery(Guid Id) : IRequest<ServiceTicketDto?>;

public class GetServiceTicketByIdQueryHandler : IRequestHandler<GetServiceTicketByIdQuery, ServiceTicketDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetServiceTicketByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceTicketDto?> Handle(GetServiceTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<ServiceTicket>().Query()
            .Include(x => x.PropertyUnit)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<ServiceTicketDto>(item);
    }
}
