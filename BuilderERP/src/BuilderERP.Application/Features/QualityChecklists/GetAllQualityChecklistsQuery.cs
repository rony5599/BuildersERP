using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.QualityChecklists;

public record GetAllQualityChecklistsQuery(long? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<QualityChecklistDto>>;

public class GetAllQualityChecklistsQueryHandler : IRequestHandler<GetAllQualityChecklistsQuery, PagedResult<QualityChecklistDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllQualityChecklistsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<QualityChecklistDto>> Handle(GetAllQualityChecklistsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<QualityChecklist>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var checklists = await query
            .OrderByDescending(x => x.ChecklistDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<QualityChecklistDto>>(checklists);
        return new PagedResult<QualityChecklistDto>(items, totalCount, page, pageSize);
    }
}
