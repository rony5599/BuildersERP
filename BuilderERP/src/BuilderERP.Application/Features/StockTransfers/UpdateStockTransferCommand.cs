using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Stocks;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockTransfers;

public record UpdateStockTransferCommand(UpdateStockTransferDto Dto) : IRequest<bool>;

public class UpdateStockTransferCommandHandler : IRequestHandler<UpdateStockTransferCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStockTransferCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateStockTransferCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<StockTransfer>();
        var transfer = await repository.GetByIdAsync(request.Dto.Id);
        if (transfer is null)
        {
            return false;
        }

        var previousMaterialId = transfer.MaterialId;
        var previousFromWarehouseId = transfer.FromWarehouseId;
        var previousToWarehouseId = transfer.ToWarehouseId;
        var previousQuantity = transfer.Quantity;

        transfer.TransferNumber = request.Dto.TransferNumber;
        transfer.TransferDate = request.Dto.TransferDate;
        transfer.Quantity = request.Dto.Quantity;
        transfer.Remarks = request.Dto.Remarks;
        transfer.MaterialId = request.Dto.MaterialId;
        transfer.FromWarehouseId = request.Dto.FromWarehouseId;
        transfer.ToWarehouseId = request.Dto.ToWarehouseId;

        repository.Update(transfer);

        // Reverse the previous effect and reapply the new one, netted per (Material, Warehouse)
        // pair rather than applied as separate calls - a pair touched twice before SaveChanges
        // (e.g. the "from" warehouse is unchanged) would otherwise cause GetOrCreateStockAsync's
        // second lookup to miss the still-unsaved row from the first and insert a duplicate.
        var netDeltas = new Dictionary<(Guid MaterialId, Guid WarehouseId), decimal>();

        void AddDelta(Guid materialId, Guid warehouseId, decimal delta)
        {
            var key = (materialId, warehouseId);
            netDeltas[key] = netDeltas.TryGetValue(key, out var existing) ? existing + delta : delta;
        }

        AddDelta(previousMaterialId, previousFromWarehouseId, previousQuantity);
        AddDelta(previousMaterialId, previousToWarehouseId, -previousQuantity);
        AddDelta(request.Dto.MaterialId, request.Dto.FromWarehouseId, -request.Dto.Quantity);
        AddDelta(request.Dto.MaterialId, request.Dto.ToWarehouseId, request.Dto.Quantity);

        foreach (var ((materialId, warehouseId), delta) in netDeltas)
        {
            if (delta != 0)
            {
                await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, materialId, warehouseId, delta);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
