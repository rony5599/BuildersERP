using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.VendorQuotations;

public record UpdateVendorQuotationCommand(UpdateVendorQuotationDto Dto) : IRequest<bool>;

public class UpdateVendorQuotationCommandHandler : IRequestHandler<UpdateVendorQuotationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVendorQuotationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateVendorQuotationCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<VendorQuotation>();
        var quotation = await repository.GetByIdAsync(request.Dto.Id);
        if (quotation is null)
        {
            return false;
        }

        quotation.QuotationNumber = request.Dto.QuotationNumber;
        quotation.QuotationDate = request.Dto.QuotationDate;
        quotation.QuotedAmount = request.Dto.QuotedAmount;
        quotation.DeliveryDays = request.Dto.DeliveryDays;
        quotation.Status = request.Dto.Status;
        quotation.RfqId = request.Dto.RfqId;

        repository.Update(quotation);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
