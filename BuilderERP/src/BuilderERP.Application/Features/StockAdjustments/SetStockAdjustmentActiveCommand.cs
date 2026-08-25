using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockAdjustments;

public record SetStockAdjustmentActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetStockAdjustmentActiveCommandHandler : IRequestHandler<SetStockAdjustmentActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetStockAdjustmentActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetStockAdjustmentActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<StockAdjustment>();
        var adjustment = await repository.GetByIdAsync(request.Id);
        if (adjustment is null)
        {
            return false;
        }

        adjustment.IsActive = request.IsActive;
        repository.Update(adjustment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
