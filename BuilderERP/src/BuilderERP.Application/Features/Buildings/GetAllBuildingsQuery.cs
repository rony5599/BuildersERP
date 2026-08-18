using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Buildings;

public record GetAllBuildingsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<BuildingDto>>;

public class GetAllBuildingsQueryHandler : IRequestHandler<GetAllBuildingsQuery, IReadOnlyList<BuildingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBuildingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BuildingDto>> Handle(GetAllBuildingsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Building>().Query().Include(b => b.Project).AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(b => b.ProjectId == request.ProjectId.Value);
        }

        var buildings = await query.ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<BuildingDto>>(buildings);
    }
}
