using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LandDocuments;

public record UpdateLandDocumentCommand(UpdateLandDocumentDto Dto) : IRequest<bool>;

public class UpdateLandDocumentCommandHandler : IRequestHandler<UpdateLandDocumentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLandDocumentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateLandDocumentCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<LandDocument>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.DocumentNumber = request.Dto.DocumentNumber;
        item.Title = request.Dto.Title;
        item.LandDocumentType = request.Dto.LandDocumentType;
        item.MouzaName = request.Dto.MouzaName;
        item.JlNumber = request.Dto.JlNumber;
        item.KhatianNumber = request.Dto.KhatianNumber;
        item.DagNumber = request.Dto.DagNumber;
        item.AreaInDecimal = request.Dto.AreaInDecimal;
        item.AcquisitionDate = request.Dto.AcquisitionDate;
        item.Remarks = request.Dto.Remarks;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
