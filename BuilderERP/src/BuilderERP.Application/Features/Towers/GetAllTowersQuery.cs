using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Towers;

public record GetAllTowersQuery(Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<TowerDto>>;

public class GetAllTowersQueryHandler : IRequestHandler<GetAllTowersQuery, PagedResult<TowerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllTowersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<TowerDto>> Handle(GetAllTowersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Tower>().Query()
            .Include(t => t.Building).ThenInclude(b => b.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(t => t.Building.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var towers = await query
            .OrderBy(t => t.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<TowerDto>>(towers);
        return new PagedResult<TowerDto>(items, totalCount, page, pageSize);
    }
}
