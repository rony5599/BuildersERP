using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PurchaseReturns;

public record SetPurchaseReturnActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetPurchaseReturnActiveCommandHandler : IRequestHandler<SetPurchaseReturnActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetPurchaseReturnActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetPurchaseReturnActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PurchaseReturn>();
        var purchaseReturn = await repository.GetByIdAsync(request.Id);
        if (purchaseReturn is null)
        {
            return false;
        }

        purchaseReturn.IsActive = request.IsActive;
        repository.Update(purchaseReturn);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
