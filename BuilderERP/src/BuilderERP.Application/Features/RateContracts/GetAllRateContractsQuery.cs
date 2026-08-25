using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.RateContracts;

public record GetAllRateContractsQuery(long? ContractorId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<RateContractDto>>;

public class GetAllRateContractsQueryHandler : IRequestHandler<GetAllRateContractsQuery, PagedResult<RateContractDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllRateContractsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<RateContractDto>> Handle(GetAllRateContractsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<RateContract>().Query()
            .Include(x => x.Contractor)
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ContractorId.HasValue)
        {
            query = query.Where(x => x.ContractorId == request.ContractorId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var contracts = await query
            .OrderBy(x => x.ContractNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<RateContractDto>>(contracts);
        return new PagedResult<RateContractDto>(items, totalCount, page, pageSize);
    }
}
