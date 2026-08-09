using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.MaintenanceRecords;

public record UpdateMaintenanceRecordCommand(UpdateMaintenanceRecordDto Dto) : IRequest<bool>;

public class UpdateMaintenanceRecordCommandHandler : IRequestHandler<UpdateMaintenanceRecordCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMaintenanceRecordCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateMaintenanceRecordCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<MaintenanceRecord>();
        var record = await repository.GetByIdAsync(request.Dto.Id);
        if (record is null)
        {
            return false;
        }

        record.MaintenanceType = request.Dto.MaintenanceType;
        record.MaintenanceDate = request.Dto.MaintenanceDate;
        record.Description = request.Dto.Description;
        record.Cost = request.Dto.Cost;
        record.Status = request.Dto.Status;
        record.NextServiceDate = request.Dto.NextServiceDate;
        record.EquipmentId = request.Dto.EquipmentId;

        repository.Update(record);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
