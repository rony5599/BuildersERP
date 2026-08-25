using BuilderERP.Application.Common.Caching;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Receipts;

public record SetReceiptActiveCommand(long Id, bool IsActive) : IRequest<bool>, IInvalidatesFeatures
{
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["Installments", "CollectionForecast"];
}

public class SetReceiptActiveCommandHandler : IRequestHandler<SetReceiptActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetReceiptActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetReceiptActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Receipt>();
        var receipt = await repository.GetByIdAsync(request.Id);
        if (receipt is null)
        {
            return false;
        }

        receipt.IsActive = request.IsActive;
        repository.Update(receipt);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
