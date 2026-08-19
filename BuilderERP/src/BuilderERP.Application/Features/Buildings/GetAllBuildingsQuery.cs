using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Buildings;

public record GetAllBuildingsQuery(Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<BuildingDto>>;

public class GetAllBuildingsQueryHandler : IRequestHandler<GetAllBuildingsQuery, PagedResult<BuildingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBuildingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<BuildingDto>> Handle(GetAllBuildingsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Building>().Query().Include(b => b.Project).AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(b => b.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var buildings = await query
            .OrderBy(b => b.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<BuildingDto>>(buildings);
        return new PagedResult<BuildingDto>(items, totalCount, page, pageSize);
    }
}
