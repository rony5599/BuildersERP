using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockTransfers;

public record SetStockTransferActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetStockTransferActiveCommandHandler : IRequestHandler<SetStockTransferActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetStockTransferActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetStockTransferActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<StockTransfer>();
        var transfer = await repository.GetByIdAsync(request.Id);
        if (transfer is null)
        {
            return false;
        }

        transfer.IsActive = request.IsActive;
        repository.Update(transfer);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
