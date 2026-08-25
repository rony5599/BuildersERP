using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.ContractorLedgers;

public record SetContractorLedgerActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetContractorLedgerActiveCommandHandler : IRequestHandler<SetContractorLedgerActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetContractorLedgerActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetContractorLedgerActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<ContractorLedger>();
        var ledger = await repository.GetByIdAsync(request.Id);
        if (ledger is null)
        {
            return false;
        }

        ledger.IsActive = request.IsActive;
        repository.Update(ledger);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
