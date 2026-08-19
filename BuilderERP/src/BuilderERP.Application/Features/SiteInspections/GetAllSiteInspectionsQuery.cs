using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SiteInspections;

public record GetAllSiteInspectionsQuery(Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<SiteInspectionDto>>;

public class GetAllSiteInspectionsQueryHandler : IRequestHandler<GetAllSiteInspectionsQuery, PagedResult<SiteInspectionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSiteInspectionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<SiteInspectionDto>> Handle(GetAllSiteInspectionsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SiteInspection>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var inspections = await query
            .OrderByDescending(x => x.InspectionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<SiteInspectionDto>>(inspections);
        return new PagedResult<SiteInspectionDto>(items, totalCount, page, pageSize);
    }
}
