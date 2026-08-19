using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SiteVisits;

public record GetAllSiteVisitsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<SiteVisitDto>>;

public class GetAllSiteVisitsQueryHandler : IRequestHandler<GetAllSiteVisitsQuery, PagedResult<SiteVisitDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSiteVisitsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<SiteVisitDto>> Handle(GetAllSiteVisitsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SiteVisit>().Query()
            .Include(v => v.Lead)
            .Include(v => v.PropertyUnit)
            .Include(v => v.AssignedToUser)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var visits = await query
            .OrderByDescending(v => v.VisitDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<SiteVisitDto>>(visits);
        return new PagedResult<SiteVisitDto>(items, totalCount, page, pageSize);
    }
}
