using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Quotations;

public record SetQuotationActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetQuotationActiveCommandHandler : IRequestHandler<SetQuotationActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetQuotationActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetQuotationActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Quotation>();
        var quotation = await repository.GetByIdAsync(request.Id);
        if (quotation is null)
        {
            return false;
        }

        quotation.IsActive = request.IsActive;
        repository.Update(quotation);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
