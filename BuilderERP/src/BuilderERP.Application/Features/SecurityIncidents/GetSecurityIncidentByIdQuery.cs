using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SecurityIncidents;

public record GetSecurityIncidentByIdQuery(Guid Id) : IRequest<SecurityIncidentDto?>;

public class GetSecurityIncidentByIdQueryHandler : IRequestHandler<GetSecurityIncidentByIdQuery, SecurityIncidentDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSecurityIncidentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SecurityIncidentDto?> Handle(GetSecurityIncidentByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<SecurityIncident>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<SecurityIncidentDto>(item);
    }
}
