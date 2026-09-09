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
    // Approving a GRN posts received quantities onto the source order's own line items (and can flip
    // its status to PartiallyReceived/Received) via GoodsReceivePostingService, which also
    // upserts Stock and updates Material average/last purchase price - so PurchaseOrders,
    // EngineerWorkOrders, CashPurchaseOrders, Stocks and Materials caches must all be invalidated too.
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["PurchaseOrders", "EngineerWorkOrders", "CashPurchaseOrders", "Stocks", "Materials"];
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
        var dto = request.Dto;
        var repository = _unitOfWork.Repository<GoodsReceive>();
        var receive = await repository.Query()
            .Include(g => g.Details)
            .FirstOrDefaultAsync(g => g.Id == dto.Id, cancellationToken);

        if (receive is null)
        {
            return UpdateGoodsReceiveResult.NotFound;
        }

        if (receive.Status != GrnStatus.Draft)
        {
            return UpdateGoodsReceiveResult.Locked;
        }

        var isApproving = dto.Status == GrnStatus.Approved;

        PurchaseOrder? purchaseOrder = null;
        EngineerWorkOrder? engineerWorkOrder = null;
        CashPurchaseOrder? cashPurchaseOrder = null;

        if (isApproving)
        {
            switch (dto.SourceType)
            {
                case GrnSourceType.PurchaseOrder:
                    purchaseOrder = await _unitOfWork.Repository<PurchaseOrder>().Query()
                        .Include(o => o.Details)
                        .Include(o => o.VendorQuotation)
                        .FirstOrDefaultAsync(o => o.Id == dto.PurchaseOrderId, cancellationToken);
                    if (purchaseOrder is null)
                    {
                        return UpdateGoodsReceiveResult.NotFound;
                    }
                    foreach (var detail in dto.Details)
                    {
                        var poDetail = purchaseOrder.Details.FirstOrDefault(d => d.Id == detail.PurchaseOrderDetailId);
                        if (poDetail is null)
                        {
                            continue;
                        }
                        if (IsOverReceipt(detail.ReceivedQuantity, poDetail.OrderedQuantity, poDetail.ReceivedQuantity, _settings.OverReceiptTolerancePercent))
                        {
                            return UpdateGoodsReceiveResult.OverReceipt;
                        }
                    }
                    break;

                case GrnSourceType.EngineerWorkOrder:
                    engineerWorkOrder = await _unitOfWork.Repository<EngineerWorkOrder>().Query()
                        .Include(o => o.Details)
                        .FirstOrDefaultAsync(o => o.Id == dto.EngineerWorkOrderId, cancellationToken);
                    if (engineerWorkOrder is null)
                    {
                        return UpdateGoodsReceiveResult.NotFound;
                    }
                    foreach (var detail in dto.Details)
                    {
                        var ewoDetail = engineerWorkOrder.Details.FirstOrDefault(d => d.Id == detail.EngineerWorkOrderDetailId);
                        if (ewoDetail is null)
                        {
                            continue;
                        }
                        if (IsOverReceipt(detail.ReceivedQuantity, ewoDetail.Qty, ewoDetail.ReceivedQuantity, _settings.OverReceiptTolerancePercent))
                        {
                            return UpdateGoodsReceiveResult.OverReceipt;
                        }
                    }
                    break;

                case GrnSourceType.CashPurchaseOrder:
                    cashPurchaseOrder = await _unitOfWork.Repository<CashPurchaseOrder>().Query()
                        .Include(o => o.Details)
                        .FirstOrDefaultAsync(o => o.Id == dto.CashPurchaseOrderId, cancellationToken);
                    if (cashPurchaseOrder is null)
                    {
                        return UpdateGoodsReceiveResult.NotFound;
                    }
                    foreach (var detail in dto.Details)
                    {
                        var cpoDetail = cashPurchaseOrder.Details.FirstOrDefault(d => d.Id == detail.CashPurchaseOrderDetailId);
                        if (cpoDetail is null)
                        {
                            continue;
                        }
                        if (IsOverReceipt(detail.ReceivedQuantity, cpoDetail.OrderedQuantity, cpoDetail.ReceivedQuantity, _settings.OverReceiptTolerancePercent))
                        {
                            return UpdateGoodsReceiveResult.OverReceipt;
                        }
                    }
                    break;
            }
        }

        receive.GrnNumber = dto.GrnNumber;
        receive.ReceivedDate = dto.ReceivedDate;
        receive.Remarks = dto.Remarks;
        receive.SourceType = dto.SourceType;
        receive.PurchaseOrderId = dto.PurchaseOrderId;
        receive.EngineerWorkOrderId = dto.EngineerWorkOrderId;
        receive.CashPurchaseOrderId = dto.CashPurchaseOrderId;
        receive.WarehouseId = dto.WarehouseId;
        receive.Status = dto.Status;

        var detailRepository = _unitOfWork.Repository<GoodsReceiveDetail>();
        foreach (var detail in receive.Details.ToList())
        {
            detailRepository.Remove(detail);
        }

        receive.Details.Clear();

        var newDetails = new List<GoodsReceiveDetail>();
        decimal receivedAmount = 0;
        foreach (var detail in dto.Details)
        {
            var amounts = LineItemCalculator.Calculate(detail.ReceivedQuantity, detail.UnitPrice, 0, detail.VatPercent, detail.TaxPercent);
            var newDetail = new GoodsReceiveDetail
            {
                GoodsReceiveId = receive.Id,
                PurchaseOrderDetailId = detail.PurchaseOrderDetailId,
                EngineerWorkOrderDetailId = detail.EngineerWorkOrderDetailId,
                CashPurchaseOrderDetailId = detail.CashPurchaseOrderDetailId,
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

        if (isApproving)
        {
            if (purchaseOrder is not null)
            {
                await GoodsReceivePostingService.ApplyForPurchaseOrderAsync(_unitOfWork, receive, newDetails, purchaseOrder, _settings.OverReceiptTolerancePercent, cancellationToken);
            }
            else if (engineerWorkOrder is not null)
            {
                await GoodsReceivePostingService.ApplyForEngineerWorkOrderAsync(_unitOfWork, receive, newDetails, engineerWorkOrder, _settings.OverReceiptTolerancePercent, cancellationToken);
            }
            else if (cashPurchaseOrder is not null)
            {
                await GoodsReceivePostingService.ApplyForCashPurchaseOrderAsync(_unitOfWork, receive, newDetails, cashPurchaseOrder, _settings.OverReceiptTolerancePercent, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return UpdateGoodsReceiveResult.Success;
    }

    private static bool IsOverReceipt(decimal receivedQuantity, decimal orderedQuantity, decimal alreadyReceivedQuantity, decimal tolerancePercent)
    {
        var remaining = orderedQuantity - alreadyReceivedQuantity;
        var allowedMax = remaining * (1 + tolerancePercent / 100m);
        return receivedQuantity > allowedMax;
    }
}
