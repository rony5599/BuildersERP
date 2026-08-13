using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.ApartmentMaintenances;

public record UpdateApartmentMaintenanceCommand(UpdateApartmentMaintenanceDto Dto) : IRequest<bool>;

public class UpdateApartmentMaintenanceCommandHandler : IRequestHandler<UpdateApartmentMaintenanceCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateApartmentMaintenanceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateApartmentMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<ApartmentMaintenance>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.MaintenanceNumber = request.Dto.MaintenanceNumber;
        item.MaintenanceType = request.Dto.MaintenanceType;
        item.ScheduledDate = request.Dto.ScheduledDate;
        item.CompletedDate = request.Dto.CompletedDate;
        item.Cost = request.Dto.Cost;
        item.Status = request.Dto.Status;
        item.AssignedTo = request.Dto.AssignedTo;
        item.Remarks = request.Dto.Remarks;
        item.PropertyUnitId = request.Dto.PropertyUnitId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
