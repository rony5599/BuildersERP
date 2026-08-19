using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.MaintenanceRequests;

public record GetAllMaintenanceRequestsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<MaintenanceRequestDto>>;

public class GetAllMaintenanceRequestsQueryHandler : IRequestHandler<GetAllMaintenanceRequestsQuery, PagedResult<MaintenanceRequestDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllMaintenanceRequestsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<MaintenanceRequestDto>> Handle(GetAllMaintenanceRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<MaintenanceRequest>().Query()
            .Include(x => x.PropertyUnit)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query
            .OrderBy(x => x.RequestNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<MaintenanceRequestDto>>(results);
        return new PagedResult<MaintenanceRequestDto>(items, totalCount, page, pageSize);
    }
}
