using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Stocks;

internal static class StockSync
{
    public static async Task<Stock> GetOrCreateStockAsync(IUnitOfWork unitOfWork, Guid materialId, Guid warehouseId)
    {
        var repository = unitOfWork.Repository<Stock>();
        var stock = await repository.Query()
            .FirstOrDefaultAsync(s => s.MaterialId == materialId && s.WarehouseId == warehouseId);

        if (stock is null)
        {
            stock = new Stock
            {
                MaterialId = materialId,
                WarehouseId = warehouseId,
                QuantityOnHand = 0
            };
            await repository.AddAsync(stock);
        }

        return stock;
    }

    public static async Task ApplyQuantityDeltaAsync(IUnitOfWork unitOfWork, Guid materialId, Guid warehouseId, decimal quantityDelta)
    {
        var stock = await GetOrCreateStockAsync(unitOfWork, materialId, warehouseId);

        // Stock is already tracked by EF Core (either just added, or loaded via Query()),
        // so mutating it here is enough - calling repository.Update() on a still-Added
        // entity would flip it to Modified and drop the pending insert.
        stock.QuantityOnHand += quantityDelta;
    }

    public static async Task ApplyQuantityDeltaAsync(
        IUnitOfWork unitOfWork,
        Guid materialId,
        Guid warehouseId,
        decimal quantityDelta,
        InventoryTransactionType transactionType,
        string documentNumber)
    {
        var stock = await GetOrCreateStockAsync(unitOfWork, materialId, warehouseId);
        stock.QuantityOnHand += quantityDelta;

        var material = await unitOfWork.Repository<Material>().GetByIdAsync(materialId);
        var unitCost = material?.AveragePurchasePrice ?? 0;

        await unitOfWork.Repository<InventoryTransaction>().AddAsync(new InventoryTransaction
        {
            TransactionType = transactionType,
            DocumentNumber = documentNumber,
            MaterialId = materialId,
            WarehouseId = warehouseId,
            QuantityIn = quantityDelta > 0 ? quantityDelta : 0,
            QuantityOut = quantityDelta < 0 ? -quantityDelta : 0,
            BalanceQuantity = stock.QuantityOnHand,
            UnitCost = unitCost,
            TotalCost = Math.Abs(quantityDelta) * unitCost
        });
    }
}
