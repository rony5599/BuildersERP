using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PurchaseRequisitions;

public record SetPurchaseRequisitionActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetPurchaseRequisitionActiveCommandHandler : IRequestHandler<SetPurchaseRequisitionActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetPurchaseRequisitionActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetPurchaseRequisitionActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PurchaseRequisition>();
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
