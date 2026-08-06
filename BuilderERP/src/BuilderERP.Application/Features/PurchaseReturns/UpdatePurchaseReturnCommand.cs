using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PurchaseReturns;

public record UpdatePurchaseReturnCommand(UpdatePurchaseReturnDto Dto) : IRequest<bool>;

public class UpdatePurchaseReturnCommandHandler : IRequestHandler<UpdatePurchaseReturnCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePurchaseReturnCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdatePurchaseReturnCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PurchaseReturn>();
        var purchaseReturn = await repository.GetByIdAsync(request.Dto.Id);
        if (purchaseReturn is null)
        {
            return false;
        }

        purchaseReturn.ReturnNumber = request.Dto.ReturnNumber;
        purchaseReturn.ReturnDate = request.Dto.ReturnDate;
        purchaseReturn.ReturnAmount = request.Dto.ReturnAmount;
        purchaseReturn.Reason = request.Dto.Reason;
        purchaseReturn.Status = request.Dto.Status;
        purchaseReturn.GoodsReceiveId = request.Dto.GoodsReceiveId;

        repository.Update(purchaseReturn);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
