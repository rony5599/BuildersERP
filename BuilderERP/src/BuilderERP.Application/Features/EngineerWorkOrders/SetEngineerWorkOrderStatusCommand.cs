using BuilderERP.Application.Common.Caching;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.EngineerWorkOrders;

public record SetEngineerWorkOrderStatusCommand(
    long Id,
    EngineerWorkOrderStatus ExpectedStatus,
    EngineerWorkOrderStatus NewStatus,
    string? ModifiedBy,
    string? RejectionReason = null) : IRequest<bool>, IInvalidatesFeatures
{
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["EwoBills"];
}

public class SetEngineerWorkOrderStatusCommandHandler : IRequestHandler<SetEngineerWorkOrderStatusCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetEngineerWorkOrderStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetEngineerWorkOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<EngineerWorkOrder>();
        var workOrder = await repository.GetByIdAsync(request.Id);
        if (workOrder is null || workOrder.Status != request.ExpectedStatus || !workOrder.IsLatestRevision)
        {
            return false;
        }

        workOrder.Status = request.NewStatus;
        workOrder.ModifiedAt = DateTime.UtcNow;
        workOrder.ModifiedBy = request.ModifiedBy;
        if (!string.IsNullOrWhiteSpace(request.RejectionReason)) { workOrder.RejectionReason = request.RejectionReason.Trim(); workOrder.RejectedBy = request.ModifiedBy; workOrder.RejectedAt = DateTime.UtcNow; }
        repository.Update(workOrder);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
