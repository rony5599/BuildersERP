using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SiteInspections;

public record GetSiteInspectionByIdQuery(long Id) : IRequest<SiteInspectionDto?>;

public class GetSiteInspectionByIdQueryHandler : IRequestHandler<GetSiteInspectionByIdQuery, SiteInspectionDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSiteInspectionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SiteInspectionDto?> Handle(GetSiteInspectionByIdQuery request, CancellationToken cancellationToken)
    {
        var inspection = await _unitOfWork.Repository<SiteInspection>().GetByIdAsync(request.Id);
        return inspection is null ? null : _mapper.Map<SiteInspectionDto>(inspection);
    }
}
