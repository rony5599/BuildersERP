using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Warehouses;

public record GetAllWarehousesQuery(Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<WarehouseDto>>;

public class GetAllWarehousesQueryHandler : IRequestHandler<GetAllWarehousesQuery, PagedResult<WarehouseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllWarehousesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<WarehouseDto>> Handle(GetAllWarehousesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Warehouse>().Query()
            .Include(w => w.Branch)
            .Include(w => w.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(w => w.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var warehouses = await query
            .OrderBy(w => w.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<WarehouseDto>>(warehouses);
        return new PagedResult<WarehouseDto>(items, totalCount, page, pageSize);
    }
}
