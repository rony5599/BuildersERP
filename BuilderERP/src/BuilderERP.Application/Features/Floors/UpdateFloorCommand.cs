using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Floors;

public record UpdateFloorCommand(UpdateFloorDto Dto) : IRequest<bool>;

public class UpdateFloorCommandHandler : IRequestHandler<UpdateFloorCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFloorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateFloorCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Floor>();
        var floor = await repository.GetByIdAsync(request.Dto.Id);
        if (floor is null)
        {
            return false;
        }

        floor.Name = request.Dto.Name;
        floor.FloorNumber = request.Dto.FloorNumber;
        floor.TowerId = request.Dto.TowerId;

        repository.Update(floor);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
