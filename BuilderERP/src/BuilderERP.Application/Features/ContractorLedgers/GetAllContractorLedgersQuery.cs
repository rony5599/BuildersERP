using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.ContractorLedgers;

public record GetAllContractorLedgersQuery(Guid? ContractorId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<ContractorLedgerDto>>;

public class GetAllContractorLedgersQueryHandler : IRequestHandler<GetAllContractorLedgersQuery, PagedResult<ContractorLedgerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllContractorLedgersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<ContractorLedgerDto>> Handle(GetAllContractorLedgersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<ContractorLedger>().Query()
            .Include(x => x.Contractor)
            .AsQueryable();

        if (request.ContractorId.HasValue)
        {
            query = query.Where(x => x.ContractorId == request.ContractorId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var ledgers = await query
            .OrderBy(x => x.TransactionDate)
            .ThenBy(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<ContractorLedgerDto>>(ledgers);
        return new PagedResult<ContractorLedgerDto>(items, totalCount, page, pageSize);
    }
}
