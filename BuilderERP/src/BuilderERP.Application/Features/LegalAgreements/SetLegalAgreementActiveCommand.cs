using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LegalAgreements;

public record SetLegalAgreementActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetLegalAgreementActiveCommandHandler : IRequestHandler<SetLegalAgreementActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetLegalAgreementActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetLegalAgreementActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<LegalAgreement>();
        var item = await repository.GetByIdAsync(request.Id);
        if (item is null)
        {
            return false;
        }

        item.IsActive = request.IsActive;
        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
