using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.VendorQuotations;

public record SetVendorQuotationActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetVendorQuotationActiveCommandHandler : IRequestHandler<SetVendorQuotationActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetVendorQuotationActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetVendorQuotationActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<VendorQuotation>();
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
