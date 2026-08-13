using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.ApartmentMaintenances;

public record GetAllApartmentMaintenancesQuery : IRequest<IReadOnlyList<ApartmentMaintenanceDto>>;

public class GetAllApartmentMaintenancesQueryHandler : IRequestHandler<GetAllApartmentMaintenancesQuery, IReadOnlyList<ApartmentMaintenanceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllApartmentMaintenancesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ApartmentMaintenanceDto>> Handle(GetAllApartmentMaintenancesQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<ApartmentMaintenance>().Query()
            .Include(x => x.PropertyUnit)
            .OrderBy(x => x.MaintenanceNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ApartmentMaintenanceDto>>(items);
    }
}
