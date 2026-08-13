using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SecurityIncidents;

public record GetAllSecurityIncidentsQuery : IRequest<IReadOnlyList<SecurityIncidentDto>>;

public class GetAllSecurityIncidentsQueryHandler : IRequestHandler<GetAllSecurityIncidentsQuery, IReadOnlyList<SecurityIncidentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSecurityIncidentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SecurityIncidentDto>> Handle(GetAllSecurityIncidentsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<SecurityIncident>().Query()
            .Include(x => x.Project)
            .OrderBy(x => x.IncidentNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SecurityIncidentDto>>(items);
    }
}
