using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.GoodsReceives;

public record CreateGoodsReceiveCommand(CreateGoodsReceiveDto Dto) : IRequest<Guid>;

public class CreateGoodsReceiveCommandHandler : IRequestHandler<CreateGoodsReceiveCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateGoodsReceiveCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateGoodsReceiveCommand request, CancellationToken cancellationToken)
    {
        var receive = _mapper.Map<GoodsReceive>(request.Dto);
        await _unitOfWork.Repository<GoodsReceive>().AddAsync(receive);

        await GoodsReceivePurchaseOrderSync.ApplyAsync(_unitOfWork, receive.PurchaseOrderId, receive.ReceivedAmount);

        await _unitOfWork.SaveChangesAsync();

        return receive.Id;
    }
}

internal static class GoodsReceivePurchaseOrderSync
{
    public static async Task ApplyAsync(IUnitOfWork unitOfWork, Guid purchaseOrderId, decimal receivedAmountDelta)
    {
        var repository = unitOfWork.Repository<PurchaseOrder>();
        var order = await repository.GetByIdAsync(purchaseOrderId);
        if (order is null)
        {
            return;
        }

        order.ReceivedAmount += receivedAmountDelta;

        order.Status = order.ReceivedAmount >= order.TotalAmount
            ? PurchaseOrderStatus.Received
            : order.ReceivedAmount > 0
                ? PurchaseOrderStatus.PartiallyReceived
                : order.Status;

        repository.Update(order);
    }
}
