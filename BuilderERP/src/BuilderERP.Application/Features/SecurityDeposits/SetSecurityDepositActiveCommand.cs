using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SecurityDeposits;

public record SetSecurityDepositActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetSecurityDepositActiveCommandHandler : IRequestHandler<SetSecurityDepositActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetSecurityDepositActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetSecurityDepositActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SecurityDeposit>();
        var deposit = await repository.GetByIdAsync(request.Id);
        if (deposit is null)
        {
            return false;
        }

        deposit.IsActive = request.IsActive;
        repository.Update(deposit);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
