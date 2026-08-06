using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Stocks;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockAdjustments;

public record UpdateStockAdjustmentCommand(UpdateStockAdjustmentDto Dto) : IRequest<bool>;

public class UpdateStockAdjustmentCommandHandler : IRequestHandler<UpdateStockAdjustmentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStockAdjustmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateStockAdjustmentCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<StockAdjustment>();
        var adjustment = await repository.GetByIdAsync(request.Dto.Id);
        if (adjustment is null)
        {
            return false;
        }

        var previousMaterialId = adjustment.MaterialId;
        var previousWarehouseId = adjustment.WarehouseId;
        var previousDelta = adjustment.QuantityDelta;

        adjustment.AdjustmentNumber = request.Dto.AdjustmentNumber;
        adjustment.AdjustmentDate = request.Dto.AdjustmentDate;
        adjustment.QuantityDelta = request.Dto.QuantityDelta;
        adjustment.Reason = request.Dto.Reason;
        adjustment.MaterialId = request.Dto.MaterialId;
        adjustment.WarehouseId = request.Dto.WarehouseId;

        repository.Update(adjustment);

        if (previousMaterialId == request.Dto.MaterialId && previousWarehouseId == request.Dto.WarehouseId)
        {
            var delta = request.Dto.QuantityDelta - previousDelta;
            await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, request.Dto.MaterialId, request.Dto.WarehouseId, delta);
        }
        else
        {
            await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, previousMaterialId, previousWarehouseId, -previousDelta);
            await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, request.Dto.MaterialId, request.Dto.WarehouseId, request.Dto.QuantityDelta);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
