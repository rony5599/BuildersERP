using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Stocks;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockReturns;

public record CreateStockReturnCommand(CreateStockReturnDto Dto) : IRequest<Guid>;

public class CreateStockReturnCommandHandler : IRequestHandler<CreateStockReturnCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateStockReturnCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateStockReturnCommand request, CancellationToken cancellationToken)
    {
        var stockReturn = _mapper.Map<StockReturn>(request.Dto);
        await _unitOfWork.Repository<StockReturn>().AddAsync(stockReturn);

        await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, stockReturn.MaterialId, stockReturn.WarehouseId, stockReturn.Quantity);

        await _unitOfWork.SaveChangesAsync();

        return stockReturn.Id;
    }
}
