using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Stocks;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockReturns;

public record CreateStockReturnCommand(CreateStockReturnDto Dto) : IRequest<long>;

public class CreateStockReturnCommandHandler : IRequestHandler<CreateStockReturnCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateStockReturnCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateStockReturnCommand request, CancellationToken cancellationToken)
    {
        var stockReturn = _mapper.Map<StockReturn>(request.Dto);
        await _unitOfWork.Repository<StockReturn>().AddAsync(stockReturn);

        await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, stockReturn.MaterialId, stockReturn.WarehouseId, stockReturn.Quantity, InventoryTransactionType.SalesReturn, stockReturn.ReturnNumber);

        await _unitOfWork.SaveChangesAsync();

        return stockReturn.Id;
    }
}
