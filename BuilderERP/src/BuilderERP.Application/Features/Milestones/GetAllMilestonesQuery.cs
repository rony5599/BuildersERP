using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Milestones;

public record GetAllMilestonesQuery(long? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<MilestoneDto>>;

public class GetAllMilestonesQueryHandler : IRequestHandler<GetAllMilestonesQuery, PagedResult<MilestoneDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllMilestonesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<MilestoneDto>> Handle(GetAllMilestonesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Milestone>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var milestones = await query
            .OrderBy(x => x.TargetDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<MilestoneDto>>(milestones);
        return new PagedResult<MilestoneDto>(items, totalCount, page, pageSize);
    }
}
