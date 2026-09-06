using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CashPurchaseOrders;

public record SetCashPurchaseOrderActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetCashPurchaseOrderActiveCommandHandler : IRequestHandler<SetCashPurchaseOrderActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetCashPurchaseOrderActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetCashPurchaseOrderActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CashPurchaseOrder>();
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
