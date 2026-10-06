using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PurchaseOrders;

public record SetPurchaseOrderStatusCommand(
    long Id,
    PurchaseOrderStatus ExpectedStatus,
    PurchaseOrderStatus NewStatus,
    string? ModifiedBy,
    string? RejectionReason = null) : IRequest<bool>;

/// <summary>
/// Bumps the Purchase Orders query-cache version without changing an order.
/// This repairs a stale list left by workflow changes made before status updates
/// were routed through the application pipeline.
/// </summary>
public record InvalidatePurchaseOrderCacheCommand : IRequest<bool>;

public class InvalidatePurchaseOrderCacheCommandHandler : IRequestHandler<InvalidatePurchaseOrderCacheCommand, bool>
{
    public Task<bool> Handle(InvalidatePurchaseOrderCacheCommand request, CancellationToken cancellationToken) =>
        Task.FromResult(true);
}

public class SetPurchaseOrderStatusCommandHandler : IRequestHandler<SetPurchaseOrderStatusCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetPurchaseOrderStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetPurchaseOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PurchaseOrder>();
        var order = await repository.GetByIdAsync(request.Id);
        if (order is null || order.Status != request.ExpectedStatus)
        {
            return false;
        }

        order.Status = request.NewStatus;
        order.ModifiedAt = DateTime.UtcNow;
        order.ModifiedBy = request.ModifiedBy;
        if (!string.IsNullOrWhiteSpace(request.RejectionReason))
        {
            order.RejectionReason = request.RejectionReason.Trim();
            order.RejectedBy = request.ModifiedBy;
            order.RejectedAt = DateTime.UtcNow;
        }
        repository.Update(order);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
