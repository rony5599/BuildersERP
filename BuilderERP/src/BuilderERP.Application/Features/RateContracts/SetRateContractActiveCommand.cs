using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.RateContracts;

public record SetRateContractActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetRateContractActiveCommandHandler : IRequestHandler<SetRateContractActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetRateContractActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetRateContractActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<RateContract>();
        var contract = await repository.GetByIdAsync(request.Id);
        if (contract is null)
        {
            return false;
        }

        contract.IsActive = request.IsActive;
        repository.Update(contract);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
