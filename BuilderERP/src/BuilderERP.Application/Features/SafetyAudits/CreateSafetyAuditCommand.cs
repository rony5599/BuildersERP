using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SafetyAudits;

public record CreateSafetyAuditCommand(CreateSafetyAuditDto Dto) : IRequest<long>;

public class CreateSafetyAuditCommandHandler : IRequestHandler<CreateSafetyAuditCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSafetyAuditCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateSafetyAuditCommand request, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<SafetyAudit>(request.Dto);
        await _unitOfWork.Repository<SafetyAudit>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return item.Id;
    }
}
