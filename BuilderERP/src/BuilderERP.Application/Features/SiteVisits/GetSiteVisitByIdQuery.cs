using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SiteVisits;

public record GetSiteVisitByIdQuery(long Id) : IRequest<SiteVisitDto?>;

public class GetSiteVisitByIdQueryHandler : IRequestHandler<GetSiteVisitByIdQuery, SiteVisitDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSiteVisitByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SiteVisitDto?> Handle(GetSiteVisitByIdQuery request, CancellationToken cancellationToken)
    {
        var visit = await _unitOfWork.Repository<SiteVisit>().GetByIdAsync(request.Id);
        return visit is null ? null : _mapper.Map<SiteVisitDto>(visit);
    }
}
