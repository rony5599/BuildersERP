using BuilderERP.Application.Common.Caching;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CashPurchaseOrders;

public record SetCashPurchaseOrderActiveCommand(long Id, bool IsActive) : IRequest<bool>, IInvalidatesFeatures
{
    // Also read by other features' cached queries: the billable-CPO dropdown and billed quantities are cached under CashPoBills; the requester ledger is cached under RequesterLedger.
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["CashPoBills", "RequesterLedger"];
}

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
