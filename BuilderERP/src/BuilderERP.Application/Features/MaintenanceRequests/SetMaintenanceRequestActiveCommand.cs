using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.MaintenanceRequests;

public record SetMaintenanceRequestActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetMaintenanceRequestActiveCommandHandler : IRequestHandler<SetMaintenanceRequestActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetMaintenanceRequestActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetMaintenanceRequestActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<MaintenanceRequest>();
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
