using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.MaterialInspections;

public record UpdateMaterialInspectionCommand(UpdateMaterialInspectionDto Dto) : IRequest<bool>;

public class UpdateMaterialInspectionCommandHandler : IRequestHandler<UpdateMaterialInspectionCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMaterialInspectionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateMaterialInspectionCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<MaterialInspection>();
        var inspection = await repository.GetByIdAsync(request.Dto.Id);
        if (inspection is null)
        {
            return false;
        }

        inspection.ProjectId = request.Dto.ProjectId;
        inspection.MaterialId = request.Dto.MaterialId;
        inspection.InspectionDate = request.Dto.InspectionDate;
        inspection.InspectedBy = request.Dto.InspectedBy;
        inspection.Quantity = request.Dto.Quantity;
        inspection.Result = request.Dto.Result;
        inspection.Remarks = request.Dto.Remarks;

        repository.Update(inspection);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
