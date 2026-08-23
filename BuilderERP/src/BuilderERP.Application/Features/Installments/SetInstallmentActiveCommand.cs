using BuilderERP.Application.Common.Caching;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Installments;

public record SetInstallmentActiveCommand(Guid Id, bool IsActive) : IRequest<bool>, IInvalidatesFeatures
{
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["CollectionForecast"];
}

public class SetInstallmentActiveCommandHandler : IRequestHandler<SetInstallmentActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetInstallmentActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetInstallmentActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Installment>();
        var installment = await repository.GetByIdAsync(request.Id);
        if (installment is null)
        {
            return false;
        }

        installment.IsActive = request.IsActive;
        repository.Update(installment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
