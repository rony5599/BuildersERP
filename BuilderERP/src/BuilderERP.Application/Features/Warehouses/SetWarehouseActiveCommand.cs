using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Warehouses;

public record SetWarehouseActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetWarehouseActiveCommandHandler : IRequestHandler<SetWarehouseActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetWarehouseActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetWarehouseActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Warehouse>();
        var warehouse = await repository.GetByIdAsync(request.Id);
        if (warehouse is null)
        {
            return false;
        }

        warehouse.IsActive = request.IsActive;
        repository.Update(warehouse);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
