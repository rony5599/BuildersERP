using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.EquipmentRentals;

public record GetEquipmentRentalByIdQuery(Guid Id) : IRequest<EquipmentRentalDto?>;

public class GetEquipmentRentalByIdQueryHandler : IRequestHandler<GetEquipmentRentalByIdQuery, EquipmentRentalDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEquipmentRentalByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EquipmentRentalDto?> Handle(GetEquipmentRentalByIdQuery request, CancellationToken cancellationToken)
    {
        var rental = await _unitOfWork.Repository<EquipmentRental>().GetByIdAsync(request.Id);
        return rental is null ? null : _mapper.Map<EquipmentRentalDto>(rental);
    }
}
