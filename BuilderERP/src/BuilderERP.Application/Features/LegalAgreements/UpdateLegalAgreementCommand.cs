using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LegalAgreements;

public record UpdateLegalAgreementCommand(UpdateLegalAgreementDto Dto) : IRequest<bool>;

public class UpdateLegalAgreementCommandHandler : IRequestHandler<UpdateLegalAgreementCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLegalAgreementCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateLegalAgreementCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<LegalAgreement>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.AgreementNumber = request.Dto.AgreementNumber;
        item.Title = request.Dto.Title;
        item.AgreementType = request.Dto.AgreementType;
        item.PartyName = request.Dto.PartyName;
        item.EffectiveDate = request.Dto.EffectiveDate;
        item.ExpiryDate = request.Dto.ExpiryDate;
        item.Status = request.Dto.Status;
        item.Remarks = request.Dto.Remarks;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
