using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SiteVisits;

public record GetAllSiteVisitsQuery : IRequest<IReadOnlyList<SiteVisitDto>>;

public class GetAllSiteVisitsQueryHandler : IRequestHandler<GetAllSiteVisitsQuery, IReadOnlyList<SiteVisitDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSiteVisitsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SiteVisitDto>> Handle(GetAllSiteVisitsQuery request, CancellationToken cancellationToken)
    {
        var visits = await _unitOfWork.Repository<SiteVisit>().Query()
            .Include(v => v.Lead)
            .Include(v => v.PropertyUnit)
            .Include(v => v.AssignedToUser)
            .OrderByDescending(v => v.VisitDate)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SiteVisitDto>>(visits);
    }
}
