using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.ParkingSlots;

public record GetAllParkingSlotsQuery : IRequest<IReadOnlyList<ParkingSlotDto>>;

public class GetAllParkingSlotsQueryHandler : IRequestHandler<GetAllParkingSlotsQuery, IReadOnlyList<ParkingSlotDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllParkingSlotsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ParkingSlotDto>> Handle(GetAllParkingSlotsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<ParkingSlot>().Query()
            .Include(x => x.Project)
            .OrderBy(x => x.SlotNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ParkingSlotDto>>(items);
    }
}
