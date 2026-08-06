using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Quotations;

public record UpdateQuotationCommand(UpdateQuotationDto Dto) : IRequest<bool>;

public class UpdateQuotationCommandHandler : IRequestHandler<UpdateQuotationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateQuotationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateQuotationCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Quotation>();
        var quotation = await repository.GetByIdAsync(request.Dto.Id);
        if (quotation is null)
        {
            return false;
        }

        quotation.QuotedPrice = request.Dto.QuotedPrice;
        quotation.ValidUntil = request.Dto.ValidUntil;
        quotation.Status = request.Dto.Status;
        quotation.CustomerId = request.Dto.CustomerId;
        quotation.PropertyUnitId = request.Dto.PropertyUnitId;

        repository.Update(quotation);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
