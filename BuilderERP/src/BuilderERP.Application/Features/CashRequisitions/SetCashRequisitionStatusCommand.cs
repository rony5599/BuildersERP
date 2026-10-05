using BuilderERP.Application.Common.Caching;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CashRequisitions;

public record SetCashRequisitionStatusCommand(long Id, RequisitionStatus ExpectedStatus, RequisitionStatus NewStatus, string? ModifiedBy)
    : IRequest<bool>, IInvalidatesFeatures
{
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["RequesterLedger", "CashDisbursements"];
}

public class SetCashRequisitionStatusCommandHandler : IRequestHandler<SetCashRequisitionStatusCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public SetCashRequisitionStatusCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<bool> Handle(SetCashRequisitionStatusCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CashRequisition>();
        var requisition = await repository.GetByIdAsync(request.Id);
        if (requisition is null || requisition.Status != request.ExpectedStatus) return false;
        requisition.Status = request.NewStatus;
        requisition.ModifiedAt = DateTime.UtcNow;
        requisition.ModifiedBy = request.ModifiedBy;
        repository.Update(requisition);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
