using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DelayEvents;

public record GetAllDelayEventsQuery(Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<DelayEventDto>>;

public class GetAllDelayEventsQueryHandler : IRequestHandler<GetAllDelayEventsQuery, PagedResult<DelayEventDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDelayEventsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<DelayEventDto>> Handle(GetAllDelayEventsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<DelayEvent>().Query()
            .Include(x => x.Project)
            .Include(x => x.WbsTask)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var delayEvents = await query
            .OrderByDescending(x => x.ReportedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<DelayEventDto>>(delayEvents);
        return new PagedResult<DelayEventDto>(items, totalCount, page, pageSize);
    }
}
