using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SafetyAudits;

public record GetSafetyAuditByIdQuery(Guid Id) : IRequest<SafetyAuditDto?>;

public class GetSafetyAuditByIdQueryHandler : IRequestHandler<GetSafetyAuditByIdQuery, SafetyAuditDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSafetyAuditByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SafetyAuditDto?> Handle(GetSafetyAuditByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<SafetyAudit>().GetByIdAsync(request.Id);
        return item is null ? null : _mapper.Map<SafetyAuditDto>(item);
    }
}
