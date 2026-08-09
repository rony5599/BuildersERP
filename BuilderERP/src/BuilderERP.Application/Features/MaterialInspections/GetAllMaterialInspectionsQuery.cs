using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.MaterialInspections;

public record GetAllMaterialInspectionsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<MaterialInspectionDto>>;

public class GetAllMaterialInspectionsQueryHandler : IRequestHandler<GetAllMaterialInspectionsQuery, IReadOnlyList<MaterialInspectionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllMaterialInspectionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<MaterialInspectionDto>> Handle(GetAllMaterialInspectionsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<MaterialInspection>().Query()
            .Include(x => x.Project)
            .Include(x => x.Material)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var inspections = await query
            .OrderByDescending(x => x.InspectionDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<MaterialInspectionDto>>(inspections);
    }
}
