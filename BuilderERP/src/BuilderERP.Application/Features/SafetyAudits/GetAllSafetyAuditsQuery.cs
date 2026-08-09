using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SafetyAudits;

public record GetAllSafetyAuditsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<SafetyAuditDto>>;

public class GetAllSafetyAuditsQueryHandler : IRequestHandler<GetAllSafetyAuditsQuery, IReadOnlyList<SafetyAuditDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSafetyAuditsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SafetyAuditDto>> Handle(GetAllSafetyAuditsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SafetyAudit>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var items = await query
            .OrderByDescending(x => x.AuditDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<SafetyAuditDto>>(items);
    }
}
