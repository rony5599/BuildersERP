using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DelayEvents;

public record GetAllDelayEventsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<DelayEventDto>>;

public class GetAllDelayEventsQueryHandler : IRequestHandler<GetAllDelayEventsQuery, IReadOnlyList<DelayEventDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDelayEventsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DelayEventDto>> Handle(GetAllDelayEventsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<DelayEvent>().Query()
            .Include(x => x.Project)
            .Include(x => x.WbsTask)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var delayEvents = await query
            .OrderByDescending(x => x.ReportedDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<DelayEventDto>>(delayEvents);
    }
}
