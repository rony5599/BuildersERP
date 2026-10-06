using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.EngineerWorkOrderRequisitions;

public record SetEngineerWorkOrderRequisitionStatusCommand(
    long Id, RequisitionStatus ExpectedStatus, RequisitionStatus NewStatus, string? ModifiedBy, string? RejectionReason = null) : IRequest<bool>;

public class SetEngineerWorkOrderRequisitionStatusCommandHandler : IRequestHandler<SetEngineerWorkOrderRequisitionStatusCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public SetEngineerWorkOrderRequisitionStatusCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(SetEngineerWorkOrderRequisitionStatusCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<EngineerWorkOrderRequisition>();
        var requisition = await repository.GetByIdAsync(request.Id);
        if (requisition is null || requisition.Status != request.ExpectedStatus) return false;

        requisition.Status = request.NewStatus;
        requisition.ModifiedAt = DateTime.UtcNow;
        requisition.ModifiedBy = request.ModifiedBy;
        if (!string.IsNullOrWhiteSpace(request.RejectionReason)) { requisition.RejectionReason = request.RejectionReason.Trim(); requisition.RejectedBy = request.ModifiedBy; requisition.RejectedAt = DateTime.UtcNow; }
        repository.Update(requisition);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
