using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SafetyAudits;

public record GetAllSafetyAuditsQuery(long? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<SafetyAuditDto>>;

public class GetAllSafetyAuditsQueryHandler : IRequestHandler<GetAllSafetyAuditsQuery, PagedResult<SafetyAuditDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSafetyAuditsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<SafetyAuditDto>> Handle(GetAllSafetyAuditsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SafetyAudit>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.AuditDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<IReadOnlyList<SafetyAuditDto>>(items);
        return new PagedResult<SafetyAuditDto>(mapped, totalCount, page, pageSize);
    }
}
