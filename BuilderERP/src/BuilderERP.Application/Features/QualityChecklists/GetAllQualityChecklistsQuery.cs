using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.QualityChecklists;

public record GetAllQualityChecklistsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<QualityChecklistDto>>;

public class GetAllQualityChecklistsQueryHandler : IRequestHandler<GetAllQualityChecklistsQuery, IReadOnlyList<QualityChecklistDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllQualityChecklistsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<QualityChecklistDto>> Handle(GetAllQualityChecklistsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<QualityChecklist>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var checklists = await query
            .OrderByDescending(x => x.ChecklistDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<QualityChecklistDto>>(checklists);
    }
}
