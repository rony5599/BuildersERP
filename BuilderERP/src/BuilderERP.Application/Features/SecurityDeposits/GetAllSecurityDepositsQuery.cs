using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SecurityDeposits;

public record GetAllSecurityDepositsQuery(long? ContractorId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<SecurityDepositDto>>;

public class GetAllSecurityDepositsQueryHandler : IRequestHandler<GetAllSecurityDepositsQuery, PagedResult<SecurityDepositDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSecurityDepositsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<SecurityDepositDto>> Handle(GetAllSecurityDepositsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SecurityDeposit>().Query()
            .Include(x => x.Contractor)
            .Include(x => x.WorkOrder)
            .AsQueryable();

        if (request.ContractorId.HasValue)
        {
            query = query.Where(x => x.ContractorId == request.ContractorId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var deposits = await query
            .OrderByDescending(x => x.DepositDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<SecurityDepositDto>>(deposits);
        return new PagedResult<SecurityDepositDto>(items, totalCount, page, pageSize);
    }
}
