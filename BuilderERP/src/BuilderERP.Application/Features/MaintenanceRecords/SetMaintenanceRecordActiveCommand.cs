using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.MaintenanceRecords;

public record SetMaintenanceRecordActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetMaintenanceRecordActiveCommandHandler : IRequestHandler<SetMaintenanceRecordActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetMaintenanceRecordActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetMaintenanceRecordActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<MaintenanceRecord>();
        var record = await repository.GetByIdAsync(request.Id);
        if (record is null)
        {
            return false;
        }

        record.IsActive = request.IsActive;
        repository.Update(record);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
