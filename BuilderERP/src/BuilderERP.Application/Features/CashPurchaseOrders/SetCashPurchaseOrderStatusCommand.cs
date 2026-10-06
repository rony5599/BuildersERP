using BuilderERP.Application.Common.Caching;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CashPurchaseOrders;

public record SetCashPurchaseOrderStatusCommand(long Id, PurchaseOrderStatus ExpectedStatus, PurchaseOrderStatus NewStatus, string? ModifiedBy, string? RejectionReason = null)
    : IRequest<bool>, IInvalidatesFeatures
{
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["CashPoBills", "RequesterLedger", "GoodsReceives"];
}

public class SetCashPurchaseOrderStatusCommandHandler : IRequestHandler<SetCashPurchaseOrderStatusCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public SetCashPurchaseOrderStatusCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<bool> Handle(SetCashPurchaseOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CashPurchaseOrder>();
        var order = await repository.GetByIdAsync(request.Id);
        if (order is null || order.Status != request.ExpectedStatus) return false;
        order.Status = request.NewStatus;
        order.ModifiedAt = DateTime.UtcNow;
        order.ModifiedBy = request.ModifiedBy;
        if (!string.IsNullOrWhiteSpace(request.RejectionReason)) { order.RejectionReason = request.RejectionReason.Trim(); order.RejectedBy = request.ModifiedBy; order.RejectedAt = DateTime.UtcNow; }
        repository.Update(order);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
