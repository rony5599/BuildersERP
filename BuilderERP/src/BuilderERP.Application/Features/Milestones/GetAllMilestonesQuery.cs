using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Milestones;

public record GetAllMilestonesQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<MilestoneDto>>;

public class GetAllMilestonesQueryHandler : IRequestHandler<GetAllMilestonesQuery, IReadOnlyList<MilestoneDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllMilestonesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<MilestoneDto>> Handle(GetAllMilestonesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Milestone>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var milestones = await query.OrderBy(x => x.TargetDate).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<MilestoneDto>>(milestones);
    }
}
