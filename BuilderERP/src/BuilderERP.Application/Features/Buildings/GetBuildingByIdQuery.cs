using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Buildings;

public record GetBuildingByIdQuery(Guid Id) : IRequest<BuildingDto?>;

public class GetBuildingByIdQueryHandler : IRequestHandler<GetBuildingByIdQuery, BuildingDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBuildingByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BuildingDto?> Handle(GetBuildingByIdQuery request, CancellationToken cancellationToken)
    {
        var building = await _unitOfWork.Repository<Building>().GetByIdAsync(request.Id);
        return building is null ? null : _mapper.Map<BuildingDto>(building);
    }
}
