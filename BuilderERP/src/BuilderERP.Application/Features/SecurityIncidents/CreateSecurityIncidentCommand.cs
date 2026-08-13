using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SecurityIncidents;

public record CreateSecurityIncidentCommand(CreateSecurityIncidentDto Dto) : IRequest<Guid>;

public class CreateSecurityIncidentCommandHandler : IRequestHandler<CreateSecurityIncidentCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSecurityIncidentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateSecurityIncidentCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<SecurityIncident>(request.Dto);
        var repository = _unitOfWork.Repository<SecurityIncident>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
