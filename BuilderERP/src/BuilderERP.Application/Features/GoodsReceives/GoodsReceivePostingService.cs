using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.GoodsReceives;

public class OverReceiptException : Exception
{
    public OverReceiptException(string message) : base(message)
    {
    }
}

internal static class GoodsReceivePostingService
{
    /// <summary>
    /// Validates and posts a GRN transitioning into Approved: increments PO line received quantities,
    /// syncs the PO header status, upserts Stock, writes InventoryTransaction rows, and updates
    /// Material price fields + ItemPriceHistory. Throws OverReceiptException without mutating anything
    /// if any line exceeds the remaining PO quantity plus tolerance, so callers should validate before
    /// persisting any other change on the same unit of work.
    /// </summary>
    public static async Task ApplyAsync(IUnitOfWork unitOfWork, GoodsReceive receive, IReadOnlyList<GoodsReceiveDetail> details, PurchaseOrder order, decimal tolerancePercent, CancellationToken cancellationToken)
    {
        foreach (var line in details)
        {
            var poDetail = order.Details.First(d => d.Id == line.PurchaseOrderDetailId);
            var remaining = poDetail.OrderedQuantity - poDetail.ReceivedQuantity;
            var allowedMax = remaining * (1 + tolerancePercent / 100m);
            if (line.ReceivedQuantity > allowedMax)
            {
                throw new OverReceiptException(
                    $"Received quantity {line.ReceivedQuantity} exceeds the remaining ordered quantity {remaining} plus the {tolerancePercent}% tolerance.");
            }
        }

        var poDetailRepository = unitOfWork.Repository<PurchaseOrderDetail>();
        var stockRepository = unitOfWork.Repository<Stock>();
        var transactionRepository = unitOfWork.Repository<InventoryTransaction>();
        var materialRepository = unitOfWork.Repository<Material>();
        var priceHistoryRepository = unitOfWork.Repository<ItemPriceHistory>();

        var supplierId = order.VendorQuotation.SupplierId;
        decimal receivedAmountDelta = 0;

        foreach (var line in details)
        {
            var poDetail = order.Details.First(d => d.Id == line.PurchaseOrderDetailId);
            poDetail.ReceivedQuantity += line.ReceivedQuantity;
            poDetailRepository.Update(poDetail);
            receivedAmountDelta += line.LineTotal;

            var stock = await stockRepository.Query()
                .FirstOrDefaultAsync(s => s.MaterialId == line.MaterialId && s.WarehouseId == receive.WarehouseId, cancellationToken);

            if (stock is null)
            {
                stock = new Stock { MaterialId = line.MaterialId, WarehouseId = receive.WarehouseId, QuantityOnHand = 0 };
                await stockRepository.AddAsync(stock);
            }

            // Stock is already tracked by EF Core (either just added, or loaded via Query()),
            // so mutating it here is enough - calling repository.Update() on a still-Added
            // entity would flip it to Modified and drop the pending insert.
            stock.QuantityOnHand += line.ReceivedQuantity;

            await transactionRepository.AddAsync(new InventoryTransaction
            {
                TransactionType = InventoryTransactionType.GoodsReceipt,
                DocumentNumber = receive.GrnNumber,
                MaterialId = line.MaterialId,
                WarehouseId = receive.WarehouseId,
                BatchNo = line.BatchNo,
                SerialNo = line.SerialNo,
                QuantityIn = line.ReceivedQuantity,
                QuantityOut = 0,
                BalanceQuantity = stock.QuantityOnHand,
                UnitCost = line.UnitPrice,
                TotalCost = line.ReceivedQuantity * line.UnitPrice
            });

            var material = await materialRepository.GetByIdAsync(line.MaterialId);
            if (material is not null)
            {
                var previousReceivedQuantity = await unitOfWork.Repository<InventoryTransaction>().Query()
                    .Where(t => t.MaterialId == line.MaterialId && t.TransactionType == InventoryTransactionType.GoodsReceipt)
                    .SumAsync(t => (decimal?)t.QuantityIn, cancellationToken) ?? 0;

                var newTotalQuantity = previousReceivedQuantity + line.ReceivedQuantity;
                material.AveragePurchasePrice = newTotalQuantity == 0
                    ? line.UnitPrice
                    : ((material.AveragePurchasePrice * previousReceivedQuantity) + (line.UnitPrice * line.ReceivedQuantity)) / newTotalQuantity;
                material.LastPurchasePrice = line.UnitPrice;
                materialRepository.Update(material);
            }

            await priceHistoryRepository.AddAsync(new ItemPriceHistory
            {
                MaterialId = line.MaterialId,
                SupplierId = supplierId,
                EffectiveDate = receive.ReceivedDate,
                UnitPrice = line.UnitPrice,
                VatPercent = line.VatPercent,
                TaxPercent = line.TaxPercent,
                DiscountPercent = 0,
                NetPrice = line.ReceivedQuantity == 0 ? line.UnitPrice : line.LineTotal / line.ReceivedQuantity,
                MinimumOrderQuantity = 0,
                LeadTimeDays = 0,
                IsContractPrice = false,
                SourceDocumentType = "GRN",
                SourceDocumentNumber = receive.GrnNumber
            });
        }

        order.ReceivedAmount += receivedAmountDelta;
        order.Status = order.Details.All(d => d.ReceivedQuantity >= d.OrderedQuantity)
            ? PurchaseOrderStatus.Received
            : order.Details.Any(d => d.ReceivedQuantity > 0)
                ? PurchaseOrderStatus.PartiallyReceived
                : order.Status;
        unitOfWork.Repository<PurchaseOrder>().Update(order);
    }
}
