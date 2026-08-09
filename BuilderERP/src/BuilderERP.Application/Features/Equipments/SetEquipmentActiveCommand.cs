using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Equipments;

public record SetEquipmentActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetEquipmentActiveCommandHandler : IRequestHandler<SetEquipmentActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetEquipmentActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetEquipmentActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Equipment>();
        var equipment = await repository.GetByIdAsync(request.Id);
        if (equipment is null)
        {
            return false;
        }

        equipment.IsActive = request.IsActive;
        repository.Update(equipment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
