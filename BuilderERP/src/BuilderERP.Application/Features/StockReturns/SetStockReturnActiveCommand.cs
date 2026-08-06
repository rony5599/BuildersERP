using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockReturns;

public record SetStockReturnActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetStockReturnActiveCommandHandler : IRequestHandler<SetStockReturnActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetStockReturnActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetStockReturnActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<StockReturn>();
        var stockReturn = await repository.GetByIdAsync(request.Id);
        if (stockReturn is null)
        {
            return false;
        }

        stockReturn.IsActive = request.IsActive;
        repository.Update(stockReturn);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
