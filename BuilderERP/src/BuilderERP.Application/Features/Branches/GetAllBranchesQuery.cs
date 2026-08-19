using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Branches;

public record GetAllBranchesQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<BranchDto>>;

public class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, PagedResult<BranchDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBranchesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<BranchDto>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Branch>().Query().Include(b => b.Company).AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var branches = await query
            .OrderBy(b => b.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<BranchDto>>(branches);
        return new PagedResult<BranchDto>(items, totalCount, page, pageSize);
    }
}
