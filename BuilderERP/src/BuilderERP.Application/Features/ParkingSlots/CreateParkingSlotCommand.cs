using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.ParkingSlots;

public record CreateParkingSlotCommand(CreateParkingSlotDto Dto) : IRequest<Guid>;

public class CreateParkingSlotCommandHandler : IRequestHandler<CreateParkingSlotCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateParkingSlotCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateParkingSlotCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ParkingSlot>(request.Dto);
        var repository = _unitOfWork.Repository<ParkingSlot>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
