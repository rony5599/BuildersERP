using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Brokers;

public record GetAllBrokersQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<BrokerDto>>;

public class GetAllBrokersQueryHandler : IRequestHandler<GetAllBrokersQuery, PagedResult<BrokerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBrokersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<BrokerDto>> Handle(GetAllBrokersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Broker>().Query().AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var brokers = await query
            .OrderBy(b => b.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<BrokerDto>>(brokers);
        return new PagedResult<BrokerDto>(items, totalCount, page, pageSize);
    }
}
