using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.ApartmentMaintenances;

public record GetApartmentMaintenanceByIdQuery(Guid Id) : IRequest<ApartmentMaintenanceDto?>;

public class GetApartmentMaintenanceByIdQueryHandler : IRequestHandler<GetApartmentMaintenanceByIdQuery, ApartmentMaintenanceDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetApartmentMaintenanceByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApartmentMaintenanceDto?> Handle(GetApartmentMaintenanceByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<ApartmentMaintenance>().Query()
            .Include(x => x.PropertyUnit)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<ApartmentMaintenanceDto>(item);
    }
}
