using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EquipmentRentals;

public record GetAllEquipmentRentalsQuery(Guid? EquipmentId = null, Guid? ProjectId = null) : IRequest<IReadOnlyList<EquipmentRentalDto>>;

public class GetAllEquipmentRentalsQueryHandler : IRequestHandler<GetAllEquipmentRentalsQuery, IReadOnlyList<EquipmentRentalDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllEquipmentRentalsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<EquipmentRentalDto>> Handle(GetAllEquipmentRentalsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<EquipmentRental>().Query()
            .Include(x => x.Equipment)
            .Include(x => x.Supplier)
            .Include(x => x.Project)
            .AsQueryable();

        if (request.EquipmentId.HasValue)
        {
            query = query.Where(x => x.EquipmentId == request.EquipmentId.Value);
        }

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var rentals = await query
            .OrderByDescending(x => x.RentalStartDate)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<EquipmentRentalDto>>(rentals);
    }
}
