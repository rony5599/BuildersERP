using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Floors;

public record SetFloorActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetFloorActiveCommandHandler : IRequestHandler<SetFloorActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetFloorActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetFloorActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Floor>();
        var floor = await repository.GetByIdAsync(request.Id);
        if (floor is null)
        {
            return false;
        }

        floor.IsActive = request.IsActive;
        repository.Update(floor);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
