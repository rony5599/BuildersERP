using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseReturns;

public class OverReturnException : Exception
{
    public OverReturnException(string message) : base(message)
    {
    }
}

internal static class PurchaseReturnPostingService
{
    public static async Task<bool> ValidateAsync(IUnitOfWork unitOfWork, PurchaseReturn purchaseReturn, IReadOnlyList<PurchaseReturnDetail> details, CancellationToken cancellationToken)
    {
        foreach (var line in details)
        {
            var goodsReceiveDetail = await unitOfWork.Repository<GoodsReceiveDetail>().GetByIdAsync(line.GoodsReceiveDetailId);
            if (goodsReceiveDetail is null)
            {
                return false;
            }

            var alreadyReturned = await unitOfWork.Repository<PurchaseReturnDetail>().Query()
                .Where(d => d.GoodsReceiveDetailId == line.GoodsReceiveDetailId
                    && d.PurchaseReturnId != purchaseReturn.Id
                    && (d.PurchaseReturn.Status == PurchaseReturnStatus.Approved || d.PurchaseReturn.Status == PurchaseReturnStatus.Completed))
                .SumAsync(d => (decimal?)d.ReturnQuantity, cancellationToken) ?? 0;

            if (line.ReturnQuantity > goodsReceiveDetail.ReceivedQuantity - alreadyReturned)
            {
                return false;
            }
        }

        return true;
    }

    public static async Task ApplyAsync(IUnitOfWork unitOfWork, PurchaseReturn purchaseReturn, IReadOnlyList<PurchaseReturnDetail> details, CancellationToken cancellationToken)
    {
        var stockRepository = unitOfWork.Repository<Stock>();
        var transactionRepository = unitOfWork.Repository<InventoryTransaction>();
        var goodsReceiveDetailRepository = unitOfWork.Repository<GoodsReceiveDetail>();

        foreach (var line in details)
        {
            var goodsReceiveDetail = await goodsReceiveDetailRepository.Query()
                .Include(d => d.GoodsReceive)
                .FirstAsync(d => d.Id == line.GoodsReceiveDetailId, cancellationToken);

            var warehouseId = goodsReceiveDetail.GoodsReceive.WarehouseId;

            var stock = await stockRepository.Query()
                .FirstOrDefaultAsync(s => s.MaterialId == line.MaterialId && s.WarehouseId == warehouseId, cancellationToken);

            if (stock is null)
            {
                stock = new Stock { MaterialId = line.MaterialId, WarehouseId = warehouseId, QuantityOnHand = 0 };
                await stockRepository.AddAsync(stock);
            }

            // Stock is already tracked by EF Core (either just added, or loaded via Query()),
            // so mutating it here is enough - calling repository.Update() on a still-Added
            // entity would flip it to Modified and drop the pending insert.
            stock.QuantityOnHand -= line.ReturnQuantity;

            await transactionRepository.AddAsync(new InventoryTransaction
            {
                TransactionType = InventoryTransactionType.PurchaseReturn,
                DocumentNumber = purchaseReturn.ReturnNumber,
                MaterialId = line.MaterialId,
                WarehouseId = warehouseId,
                QuantityIn = 0,
                QuantityOut = line.ReturnQuantity,
                BalanceQuantity = stock.QuantityOnHand,
                UnitCost = line.UnitPrice,
                TotalCost = line.ReturnQuantity * line.UnitPrice
            });
        }
    }
}
