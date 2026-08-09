using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SiteInspections;

public record GetAllSiteInspectionsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<SiteInspectionDto>>;

public class GetAllSiteInspectionsQueryHandler : IRequestHandler<GetAllSiteInspectionsQuery, IReadOnlyList<SiteInspectionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSiteInspectionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SiteInspectionDto>> Handle(GetAllSiteInspectionsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SiteInspection>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var inspections = await query
            .OrderByDescending(x => x.InspectionDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<SiteInspectionDto>>(inspections);
    }
}
