using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Materials;

public record GetAllMaterialsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<MaterialDto>>;

public class GetAllMaterialsQueryHandler : IRequestHandler<GetAllMaterialsQuery, IReadOnlyList<MaterialDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllMaterialsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<MaterialDto>> Handle(GetAllMaterialsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Material>().Query().AsQueryable();

        if (request.ProjectId.HasValue)
        {
            var materialIdsInProject = _unitOfWork.Repository<Stock>().Query()
                .Where(s => s.Warehouse.ProjectId == request.ProjectId.Value)
                .Select(s => s.MaterialId)
                .Distinct();

            query = query.Where(m => materialIdsInProject.Contains(m.Id));
        }

        var materials = await query.ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<MaterialDto>>(materials);
    }
}
