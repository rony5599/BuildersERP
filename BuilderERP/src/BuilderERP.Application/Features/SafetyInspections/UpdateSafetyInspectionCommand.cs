using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SafetyInspections;

public record UpdateSafetyInspectionCommand(UpdateSafetyInspectionDto Dto) : IRequest<bool>;

public class UpdateSafetyInspectionCommandHandler : IRequestHandler<UpdateSafetyInspectionCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSafetyInspectionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSafetyInspectionCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SafetyInspection>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.ProjectId = request.Dto.ProjectId;
        item.InspectionDate = request.Dto.InspectionDate;
        item.InspectedBy = request.Dto.InspectedBy;
        item.Location = request.Dto.Location;
        item.Result = request.Dto.Result;
        item.Remarks = request.Dto.Remarks;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
