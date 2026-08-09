using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SafetyInspections;

public record GetAllSafetyInspectionsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<SafetyInspectionDto>>;

public class GetAllSafetyInspectionsQueryHandler : IRequestHandler<GetAllSafetyInspectionsQuery, IReadOnlyList<SafetyInspectionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSafetyInspectionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SafetyInspectionDto>> Handle(GetAllSafetyInspectionsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SafetyInspection>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var items = await query
            .OrderByDescending(x => x.InspectionDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<SafetyInspectionDto>>(items);
    }
}
