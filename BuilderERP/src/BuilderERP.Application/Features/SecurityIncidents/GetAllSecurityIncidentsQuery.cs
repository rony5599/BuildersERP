using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SecurityIncidents;

public record GetAllSecurityIncidentsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<SecurityIncidentDto>>;

public class GetAllSecurityIncidentsQueryHandler : IRequestHandler<GetAllSecurityIncidentsQuery, PagedResult<SecurityIncidentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSecurityIncidentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<SecurityIncidentDto>> Handle(GetAllSecurityIncidentsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SecurityIncident>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query
            .OrderBy(x => x.IncidentNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<SecurityIncidentDto>>(results);
        return new PagedResult<SecurityIncidentDto>(items, totalCount, page, pageSize);
    }
}
