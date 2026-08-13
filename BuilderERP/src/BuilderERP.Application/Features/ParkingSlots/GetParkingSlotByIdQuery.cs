using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.ParkingSlots;

public record GetParkingSlotByIdQuery(Guid Id) : IRequest<ParkingSlotDto?>;

public class GetParkingSlotByIdQueryHandler : IRequestHandler<GetParkingSlotByIdQuery, ParkingSlotDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetParkingSlotByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ParkingSlotDto?> Handle(GetParkingSlotByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<ParkingSlot>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<ParkingSlotDto>(item);
    }
}
