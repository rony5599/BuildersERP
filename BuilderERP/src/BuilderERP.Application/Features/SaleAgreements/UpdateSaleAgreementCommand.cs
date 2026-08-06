using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SaleAgreements;

public record UpdateSaleAgreementCommand(UpdateSaleAgreementDto Dto) : IRequest<bool>;

public class UpdateSaleAgreementCommandHandler : IRequestHandler<UpdateSaleAgreementCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSaleAgreementCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSaleAgreementCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SaleAgreement>();
        var agreement = await repository.GetByIdAsync(request.Dto.Id);
        if (agreement is null)
        {
            return false;
        }

        agreement.AgreementNumber = request.Dto.AgreementNumber;
        agreement.AgreementDate = request.Dto.AgreementDate;
        agreement.TotalSalePrice = request.Dto.TotalSalePrice;
        agreement.Status = request.Dto.Status;
        agreement.BookingId = request.Dto.BookingId;

        repository.Update(agreement);

        await SaleAgreementUnitStatusSync.ApplyAsync(_unitOfWork, agreement.BookingId, agreement.Status);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
