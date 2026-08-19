using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PropertyUnits;

public record GetAllPropertyUnitsQuery(Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<PropertyUnitDto>>;

public class GetAllPropertyUnitsQueryHandler : IRequestHandler<GetAllPropertyUnitsQuery, PagedResult<PropertyUnitDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPropertyUnitsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<PropertyUnitDto>> Handle(GetAllPropertyUnitsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PropertyUnit>().Query()
            .Include(u => u.Floor).ThenInclude(f => f.Tower).ThenInclude(t => t.Building).ThenInclude(b => b.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(u => u.Floor.Tower.Building.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var units = await query
            .OrderBy(u => u.UnitNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<PropertyUnitDto>>(units);
        return new PagedResult<PropertyUnitDto>(items, totalCount, page, pageSize);
    }
}
