using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.GoodsReceives;

public record UpdateGoodsReceiveCommand(UpdateGoodsReceiveDto Dto) : IRequest<bool>;

public class UpdateGoodsReceiveCommandHandler : IRequestHandler<UpdateGoodsReceiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGoodsReceiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateGoodsReceiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<GoodsReceive>();
        var receive = await repository.GetByIdAsync(request.Dto.Id);
        if (receive is null)
        {
            return false;
        }

        var previousAmount = receive.ReceivedAmount;
        var previousPurchaseOrderId = receive.PurchaseOrderId;

        receive.GrnNumber = request.Dto.GrnNumber;
        receive.ReceivedDate = request.Dto.ReceivedDate;
        receive.ReceivedAmount = request.Dto.ReceivedAmount;
        receive.Remarks = request.Dto.Remarks;
        receive.PurchaseOrderId = request.Dto.PurchaseOrderId;

        repository.Update(receive);

        if (previousPurchaseOrderId == request.Dto.PurchaseOrderId)
        {
            var delta = request.Dto.ReceivedAmount - previousAmount;
            await GoodsReceivePurchaseOrderSync.ApplyAsync(_unitOfWork, request.Dto.PurchaseOrderId, delta);
        }
        else
        {
            await GoodsReceivePurchaseOrderSync.ApplyAsync(_unitOfWork, previousPurchaseOrderId, -previousAmount);
            await GoodsReceivePurchaseOrderSync.ApplyAsync(_unitOfWork, request.Dto.PurchaseOrderId, request.Dto.ReceivedAmount);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
