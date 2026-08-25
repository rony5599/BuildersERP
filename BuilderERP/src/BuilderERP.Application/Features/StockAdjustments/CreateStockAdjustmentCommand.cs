using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Stocks;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockAdjustments;

public record CreateStockAdjustmentCommand(CreateStockAdjustmentDto Dto) : IRequest<long>;

public class CreateStockAdjustmentCommandHandler : IRequestHandler<CreateStockAdjustmentCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateStockAdjustmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateStockAdjustmentCommand request, CancellationToken cancellationToken)
    {
        var adjustment = _mapper.Map<StockAdjustment>(request.Dto);
        await _unitOfWork.Repository<StockAdjustment>().AddAsync(adjustment);

        await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, adjustment.MaterialId, adjustment.WarehouseId, adjustment.QuantityDelta, InventoryTransactionType.StockAdjustment, adjustment.AdjustmentNumber);

        await _unitOfWork.SaveChangesAsync();

        return adjustment.Id;
    }
}
