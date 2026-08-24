using BuilderERP.Application.Common;
using BuilderERP.Application.Common.Caching;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BuilderERP.Application.Features.GoodsReceives;

public record UpdateGoodsReceiveCommand(UpdateGoodsReceiveDto Dto) : IRequest<UpdateGoodsReceiveResult>, IInvalidatesFeatures
{
    // Approving a GRN posts received quantities onto the PO's own line items (and can flip
    // the PO's status to PartiallyReceived/Received) via GoodsReceivePostingService, which also
    // upserts Stock and updates Material average/last purchase price - so all three features'
    // cached queries must be invalidated too, not just GoodsReceives' own.
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["PurchaseOrders", "Stocks", "Materials"];
}

public enum UpdateGoodsReceiveResult
{
    Success,
    NotFound,
    Locked,
    OverReceipt
}

public class UpdateGoodsReceiveCommandHandler : IRequestHandler<UpdateGoodsReceiveCommand, UpdateGoodsReceiveResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ProcurementSettings _settings;

    public UpdateGoodsReceiveCommandHandler(IUnitOfWork unitOfWork, IOptions<ProcurementSettings> settings)
    {
        _unitOfWork = unitOfWork;
        _settings = settings.Value;
    }

    public async Task<UpdateGoodsReceiveResult> Handle(UpdateGoodsReceiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<GoodsReceive>();
        var receive = await repository.Query()
            .Include(g => g.Details)
            .FirstOrDefaultAsync(g => g.Id == request.Dto.Id, cancellationToken);

        if (receive is null)
        {
            return UpdateGoodsReceiveResult.NotFound;
        }

        if (receive.Status != GrnStatus.Draft)
        {
            return UpdateGoodsReceiveResult.Locked;
        }

        var isApproving = request.Dto.Status == GrnStatus.Approved;
        PurchaseOrder? order = null;

        if (isApproving)
        {
            order = await _unitOfWork.Repository<PurchaseOrder>().Query()
                .Include(o => o.Details)
                .Include(o => o.VendorQuotation)
                .FirstOrDefaultAsync(o => o.Id == request.Dto.PurchaseOrderId, cancellationToken);

            if (order is null)
            {
                return UpdateGoodsReceiveResult.NotFound;
            }

            foreach (var detail in request.Dto.Details)
            {
                var poDetail = order.Details.FirstOrDefault(d => d.Id == detail.PurchaseOrderDetailId);
                if (poDetail is null)
                {
                    continue;
                }

                var remaining = poDetail.OrderedQuantity - poDetail.ReceivedQuantity;
                var allowedMax = remaining * (1 + _settings.OverReceiptTolerancePercent / 100m);
                if (detail.ReceivedQuantity > allowedMax)
                {
                    return UpdateGoodsReceiveResult.OverReceipt;
                }
            }
        }

        receive.GrnNumber = request.Dto.GrnNumber;
        receive.ReceivedDate = request.Dto.ReceivedDate;
        receive.Remarks = request.Dto.Remarks;
        receive.PurchaseOrderId = request.Dto.PurchaseOrderId;
        receive.WarehouseId = request.Dto.WarehouseId;
        receive.Status = request.Dto.Status;

        var detailRepository = _unitOfWork.Repository<GoodsReceiveDetail>();
        foreach (var detail in receive.Details.ToList())
        {
            detailRepository.Remove(detail);
        }

        receive.Details.Clear();

        var newDetails = new List<GoodsReceiveDetail>();
        decimal receivedAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var amounts = LineItemCalculator.Calculate(detail.ReceivedQuantity, detail.UnitPrice, 0, detail.VatPercent, detail.TaxPercent);
            var newDetail = new GoodsReceiveDetail
            {
                GoodsReceiveId = receive.Id,
                PurchaseOrderDetailId = detail.PurchaseOrderDetailId,
                MaterialId = detail.MaterialId,
                ReceivedQuantity = detail.ReceivedQuantity,
                UnitOfMeasure = detail.UnitOfMeasure,
                UnitPrice = detail.UnitPrice,
                BatchNo = detail.BatchNo,
                SerialNo = detail.SerialNo,
                VatPercent = detail.VatPercent,
                VatAmount = amounts.VatAmount,
                TaxPercent = detail.TaxPercent,
                TaxAmount = amounts.TaxAmount,
                LineTotal = amounts.NetAmount
            };
            await detailRepository.AddAsync(newDetail);
            newDetails.Add(newDetail);
            receivedAmount += amounts.NetAmount;
        }

        receive.ReceivedAmount = receivedAmount;
        repository.Update(receive);

        if (isApproving && order is not null)
        {
            await GoodsReceivePostingService.ApplyAsync(_unitOfWork, receive, newDetails, order, _settings.OverReceiptTolerancePercent, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync();
        return UpdateGoodsReceiveResult.Success;
    }
}
