using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Buildings;

public record SetBuildingActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetBuildingActiveCommandHandler : IRequestHandler<SetBuildingActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetBuildingActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetBuildingActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Building>();
        var building = await repository.GetByIdAsync(request.Id);
        if (building is null)
        {
            return false;
        }

        building.IsActive = request.IsActive;
        repository.Update(building);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
