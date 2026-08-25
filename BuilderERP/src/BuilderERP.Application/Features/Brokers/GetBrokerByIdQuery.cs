using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Brokers;

public record GetBrokerByIdQuery(long Id) : IRequest<BrokerDto?>;

public class GetBrokerByIdQueryHandler : IRequestHandler<GetBrokerByIdQuery, BrokerDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBrokerByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BrokerDto?> Handle(GetBrokerByIdQuery request, CancellationToken cancellationToken)
    {
        var broker = await _unitOfWork.Repository<Broker>().GetByIdAsync(request.Id);
        return broker is null ? null : _mapper.Map<BrokerDto>(broker);
    }
}
