using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.ParkingSlots;

public record UpdateParkingSlotCommand(UpdateParkingSlotDto Dto) : IRequest<bool>;

public class UpdateParkingSlotCommandHandler : IRequestHandler<UpdateParkingSlotCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateParkingSlotCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateParkingSlotCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<ParkingSlot>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.SlotNumber = request.Dto.SlotNumber;
        item.SlotType = request.Dto.SlotType;
        item.Status = request.Dto.Status;
        item.AllocatedTo = request.Dto.AllocatedTo;
        item.Remarks = request.Dto.Remarks;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
