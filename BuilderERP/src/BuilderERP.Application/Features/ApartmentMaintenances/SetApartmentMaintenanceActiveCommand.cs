using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.ApartmentMaintenances;

public record SetApartmentMaintenanceActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetApartmentMaintenanceActiveCommandHandler : IRequestHandler<SetApartmentMaintenanceActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetApartmentMaintenanceActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetApartmentMaintenanceActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<ApartmentMaintenance>();
        var item = await repository.GetByIdAsync(request.Id);
        if (item is null)
        {
            return false;
        }

        item.IsActive = request.IsActive;
        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
