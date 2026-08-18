using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Brokers;

public record GetAllBrokersQuery : IRequest<IReadOnlyList<BrokerDto>>;

public class GetAllBrokersQueryHandler : IRequestHandler<GetAllBrokersQuery, IReadOnlyList<BrokerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBrokersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BrokerDto>> Handle(GetAllBrokersQuery request, CancellationToken cancellationToken)
    {
        var brokers = await _unitOfWork.Repository<Broker>().Query().ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<BrokerDto>>(brokers);
    }
}
