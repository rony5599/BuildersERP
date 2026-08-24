using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Stocks;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockTransfers;

public record CreateStockTransferCommand(CreateStockTransferDto Dto) : IRequest<Guid>;

public class CreateStockTransferCommandHandler : IRequestHandler<CreateStockTransferCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateStockTransferCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateStockTransferCommand request, CancellationToken cancellationToken)
    {
        var transfer = _mapper.Map<StockTransfer>(request.Dto);
        await _unitOfWork.Repository<StockTransfer>().AddAsync(transfer);

        await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, transfer.MaterialId, transfer.FromWarehouseId, -transfer.Quantity, InventoryTransactionType.StockTransfer, transfer.TransferNumber);
        await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, transfer.MaterialId, transfer.ToWarehouseId, transfer.Quantity, InventoryTransactionType.StockTransfer, transfer.TransferNumber);

        await _unitOfWork.SaveChangesAsync();

        return transfer.Id;
    }
}
