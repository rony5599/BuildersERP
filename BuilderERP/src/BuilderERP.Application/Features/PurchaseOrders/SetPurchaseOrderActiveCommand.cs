using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PurchaseOrders;

public record SetPurchaseOrderActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetPurchaseOrderActiveCommandHandler : IRequestHandler<SetPurchaseOrderActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetPurchaseOrderActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetPurchaseOrderActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PurchaseOrder>();
        var order = await repository.GetByIdAsync(request.Id);
        if (order is null)
        {
            return false;
        }

        order.IsActive = request.IsActive;
        repository.Update(order);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
