using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SiteInspections;

public record UpdateSiteInspectionCommand(UpdateSiteInspectionDto Dto) : IRequest<bool>;

public class UpdateSiteInspectionCommandHandler : IRequestHandler<UpdateSiteInspectionCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSiteInspectionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSiteInspectionCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SiteInspection>();
        var inspection = await repository.GetByIdAsync(request.Dto.Id);
        if (inspection is null)
        {
            return false;
        }

        inspection.ProjectId = request.Dto.ProjectId;
        inspection.InspectionDate = request.Dto.InspectionDate;
        inspection.InspectedBy = request.Dto.InspectedBy;
        inspection.Location = request.Dto.Location;
        inspection.Result = request.Dto.Result;
        inspection.Remarks = request.Dto.Remarks;

        repository.Update(inspection);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
