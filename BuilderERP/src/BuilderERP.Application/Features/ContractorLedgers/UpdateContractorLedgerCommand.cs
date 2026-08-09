using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.ContractorLedgers;

public record UpdateContractorLedgerCommand(UpdateContractorLedgerDto Dto) : IRequest<bool>;

public class UpdateContractorLedgerCommandHandler : IRequestHandler<UpdateContractorLedgerCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContractorLedgerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateContractorLedgerCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<ContractorLedger>();
        var ledger = await repository.GetByIdAsync(request.Dto.Id);
        if (ledger is null)
        {
            return false;
        }

        ledger.ContractorId = request.Dto.ContractorId;
        ledger.TransactionDate = request.Dto.TransactionDate;
        ledger.Description = request.Dto.Description;
        ledger.DebitAmount = request.Dto.DebitAmount;
        ledger.CreditAmount = request.Dto.CreditAmount;

        repository.Update(ledger);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
