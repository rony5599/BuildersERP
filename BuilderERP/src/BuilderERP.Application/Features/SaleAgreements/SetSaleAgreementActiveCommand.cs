using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SaleAgreements;

public record SetSaleAgreementActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetSaleAgreementActiveCommandHandler : IRequestHandler<SetSaleAgreementActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetSaleAgreementActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetSaleAgreementActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SaleAgreement>();
        var agreement = await repository.GetByIdAsync(request.Id);
        if (agreement is null)
        {
            return false;
        }

        agreement.IsActive = request.IsActive;
        repository.Update(agreement);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
