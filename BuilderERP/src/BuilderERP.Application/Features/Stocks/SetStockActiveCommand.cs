using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Stocks;

public record SetStockActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetStockActiveCommandHandler : IRequestHandler<SetStockActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetStockActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetStockActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Stock>();
        var stock = await repository.GetByIdAsync(request.Id);
        if (stock is null)
        {
            return false;
        }

        stock.IsActive = request.IsActive;
        repository.Update(stock);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
