using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LegalCases;

public record UpdateLegalCaseCommand(UpdateLegalCaseDto Dto) : IRequest<bool>;

public class UpdateLegalCaseCommandHandler : IRequestHandler<UpdateLegalCaseCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLegalCaseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateLegalCaseCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<LegalCase>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.CaseNumber = request.Dto.CaseNumber;
        item.CaseTitle = request.Dto.CaseTitle;
        item.CourtName = request.Dto.CourtName;
        item.CaseType = request.Dto.CaseType;
        item.FilingDate = request.Dto.FilingDate;
        item.Status = request.Dto.Status;
        item.OpposingParty = request.Dto.OpposingParty;
        item.LawyerName = request.Dto.LawyerName;
        item.NextHearingDate = request.Dto.NextHearingDate;
        item.Remarks = request.Dto.Remarks;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
