using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CostCenters;

public record GetAllCostCentersQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<CostCenterDto>>;

public class GetAllCostCentersQueryHandler : IRequestHandler<GetAllCostCentersQuery, PagedResult<CostCenterDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCostCentersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<CostCenterDto>> Handle(GetAllCostCentersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<CostCenter>().Query().Include(c => c.Project).AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var costCenters = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<CostCenterDto>>(costCenters);
        return new PagedResult<CostCenterDto>(items, totalCount, page, pageSize);
    }
}
