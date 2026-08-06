using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Stocks;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockReturns;

public record UpdateStockReturnCommand(UpdateStockReturnDto Dto) : IRequest<bool>;

public class UpdateStockReturnCommandHandler : IRequestHandler<UpdateStockReturnCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStockReturnCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateStockReturnCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<StockReturn>();
        var stockReturn = await repository.GetByIdAsync(request.Dto.Id);
        if (stockReturn is null)
        {
            return false;
        }

        var previousMaterialId = stockReturn.MaterialId;
        var previousWarehouseId = stockReturn.WarehouseId;
        var previousQuantity = stockReturn.Quantity;

        stockReturn.ReturnNumber = request.Dto.ReturnNumber;
        stockReturn.ReturnDate = request.Dto.ReturnDate;
        stockReturn.Quantity = request.Dto.Quantity;
        stockReturn.Reason = request.Dto.Reason;
        stockReturn.MaterialId = request.Dto.MaterialId;
        stockReturn.WarehouseId = request.Dto.WarehouseId;

        repository.Update(stockReturn);

        if (previousMaterialId == request.Dto.MaterialId && previousWarehouseId == request.Dto.WarehouseId)
        {
            var delta = request.Dto.Quantity - previousQuantity;
            await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, request.Dto.MaterialId, request.Dto.WarehouseId, delta);
        }
        else
        {
            await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, previousMaterialId, previousWarehouseId, -previousQuantity);
            await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, request.Dto.MaterialId, request.Dto.WarehouseId, request.Dto.Quantity);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
