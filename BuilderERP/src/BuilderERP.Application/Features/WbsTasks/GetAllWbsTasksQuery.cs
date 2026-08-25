using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.WbsTasks;

public record GetAllWbsTasksQuery(long? ProjectId = null, long? PropertyUnitId = null) : IRequest<IReadOnlyList<WbsTaskDto>>;

public class GetAllWbsTasksQueryHandler : IRequestHandler<GetAllWbsTasksQuery, IReadOnlyList<WbsTaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllWbsTasksQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<WbsTaskDto>> Handle(GetAllWbsTasksQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<WbsTask>().Query()
            .Include(x => x.Project)
            .Include(x => x.Parent)
            .Include(x => x.PropertyUnit)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        if (request.PropertyUnitId.HasValue)
        {
            query = query.Where(x => x.PropertyUnitId == request.PropertyUnitId.Value);
        }

        var wbsTasks = await query.OrderBy(x => x.Sequence).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<WbsTaskDto>>(wbsTasks);
    }
}
