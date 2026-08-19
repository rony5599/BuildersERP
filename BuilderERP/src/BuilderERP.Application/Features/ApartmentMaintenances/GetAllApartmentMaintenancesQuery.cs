using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.ApartmentMaintenances;

public record GetAllApartmentMaintenancesQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<ApartmentMaintenanceDto>>;

public class GetAllApartmentMaintenancesQueryHandler : IRequestHandler<GetAllApartmentMaintenancesQuery, PagedResult<ApartmentMaintenanceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllApartmentMaintenancesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<ApartmentMaintenanceDto>> Handle(GetAllApartmentMaintenancesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<ApartmentMaintenance>().Query()
            .Include(x => x.PropertyUnit)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query
            .OrderBy(x => x.MaintenanceNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<ApartmentMaintenanceDto>>(results);
        return new PagedResult<ApartmentMaintenanceDto>(items, totalCount, page, pageSize);
    }
}
