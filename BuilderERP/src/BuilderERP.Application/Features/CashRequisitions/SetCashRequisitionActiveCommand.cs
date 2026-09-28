using BuilderERP.Application.Common.Caching;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CashRequisitions;

public record SetCashRequisitionActiveCommand(long Id, bool IsActive) : IRequest<bool>, IInvalidatesFeatures
{
    // Also read by other features' cached queries: the requester ledger and the disbursable-requisition dropdown are cached under RequesterLedger and CashDisbursements.
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["RequesterLedger", "CashDisbursements"];
}

public class SetCashRequisitionActiveCommandHandler : IRequestHandler<SetCashRequisitionActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetCashRequisitionActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetCashRequisitionActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CashRequisition>();
        var requisition = await repository.GetByIdAsync(request.Id);
        if (requisition is null)
        {
            return false;
        }

        requisition.IsActive = request.IsActive;
        repository.Update(requisition);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
